(() => {
    const csrfToken = document.querySelector('meta[name="csrf-token"]').content;
    const messagesEl = document.getElementById('chatMessages');
    const form = document.getElementById('chatForm');
    const input = document.getElementById('questionInput');
    const sendBtn = document.getElementById('sendBtn');
    const collectionSelect = document.getElementById('collectionSelect');
    const streamingToggle = document.getElementById('streamingToggle');
    const newConversationBtn = document.getElementById('newConversationBtn');

    let conversationId = null;

    newConversationBtn.addEventListener('click', () => {
        conversationId = null;
        messagesEl.innerHTML = '';
    });

    function scrollToBottom() {
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }

    function appendUserMessage(text) {
        const wrapper = document.createElement('div');
        wrapper.className = 'mb-3 text-end';
        const bubble = document.createElement('div');
        bubble.className = 'd-inline-block bg-primary text-white rounded p-2';
        bubble.style.maxWidth = '80%';
        bubble.style.whiteSpace = 'pre-wrap';
        bubble.textContent = text;
        wrapper.appendChild(bubble);
        messagesEl.appendChild(wrapper);
        scrollToBottom();
    }

    function createAssistantBubble() {
        const wrapper = document.createElement('div');
        wrapper.className = 'mb-3';
        const bubble = document.createElement('div');
        bubble.className = 'd-inline-block bg-white border rounded p-2';
        bubble.style.maxWidth = '80%';
        bubble.style.whiteSpace = 'pre-wrap';
        wrapper.appendChild(bubble);
        const extra = document.createElement('div');
        extra.className = 'small mt-1';
        wrapper.appendChild(extra);
        messagesEl.appendChild(wrapper);
        scrollToBottom();
        return { wrapper, bubble, extra };
    }

    function renderSources(container, sources) {
        if (!sources || sources.length === 0) return;
        const list = document.createElement('div');
        list.className = 'mt-2';
        const title = document.createElement('div');
        title.className = 'text-muted';
        title.textContent = 'Fonti:';
        list.appendChild(title);

        sources.forEach((s) => {
            const filename = s.filename ?? '';
            const page = s.pageNumber ?? s.page_number;
            const score = s.score;
            const snippet = s.snippet;

            const item = document.createElement('div');
            item.className = 'border rounded p-2 mt-1 bg-light';

            const header = document.createElement('div');
            header.textContent = `${filename}${page != null ? ' - p.' + page : ''}${score != null ? ' (score ' + Number(score).toFixed(2) + ')' : ''}`;
            header.className = 'fw-semibold';
            item.appendChild(header);

            if (snippet) {
                const snippetEl = document.createElement('div');
                snippetEl.className = 'text-muted small mt-1';
                snippetEl.textContent = snippet;
                item.appendChild(snippetEl);
            }

            list.appendChild(item);
        });

        container.appendChild(list);
    }

    function renderFeedback(container, messageId) {
        const bar = document.createElement('div');
        bar.className = 'mt-2';
        const up = document.createElement('button');
        up.type = 'button';
        up.className = 'btn btn-sm btn-outline-success me-1';
        up.textContent = '👍';
        const down = document.createElement('button');
        down.type = 'button';
        down.className = 'btn btn-sm btn-outline-danger';
        down.textContent = '👎';

        const send = (rating) => {
            up.disabled = true;
            down.disabled = true;
            fetch('/Chat/Feedback', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': csrfToken },
                body: JSON.stringify({ messageId, rating }),
            })
                .then((r) => {
                    bar.innerHTML = r.ok
                        ? '<span class="text-success small">Grazie per il feedback!</span>'
                        : '<span class="text-danger small">Errore nell\'invio del feedback.</span>';
                })
                .catch(() => {
                    bar.innerHTML = '<span class="text-danger small">Errore nell\'invio del feedback.</span>';
                });
        };

        up.addEventListener('click', () => send(1));
        down.addEventListener('click', () => send(-1));
        bar.appendChild(up);
        bar.appendChild(down);
        container.appendChild(bar);
    }

    function showError(message) {
        const div = document.createElement('div');
        div.className = 'mb-3 text-danger small';
        div.textContent = message;
        messagesEl.appendChild(div);
        scrollToBottom();
    }

    async function sendQuery(question, collectionId) {
        const { bubble, extra } = createAssistantBubble();
        bubble.textContent = 'Sto pensando...';
        try {
            const res = await fetch('/Chat/Send', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': csrfToken },
                body: JSON.stringify({ question, conversationId, collectionId }),
            });

            if (res.status === 401) {
                window.location.href = '/Account/Login?expired=true';
                return;
            }

            const data = await res.json();
            if (!res.ok) {
                bubble.closest('.mb-3').remove();
                showError(data.error || 'Errore durante la richiesta.');
                return;
            }

            conversationId = data.conversationId;
            bubble.textContent = data.answer;
            renderSources(extra, data.sources);
            if (data.messageId) {
                renderFeedback(extra, data.messageId);
            }
        } catch {
            bubble.closest('.mb-3').remove();
            showError('Impossibile contattare il server.');
        } finally {
            scrollToBottom();
        }
    }

    async function sendStream(question, collectionId) {
        const { bubble, extra } = createAssistantBubble();
        let accumulated = '';
        try {
            const res = await fetch('/Chat/Stream', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': csrfToken },
                body: JSON.stringify({ question, conversationId, collectionId }),
            });

            if (res.status === 401) {
                window.location.href = '/Account/Login?expired=true';
                return;
            }

            if (!res.ok || !res.body) {
                bubble.closest('.mb-3').remove();
                showError('Errore durante lo streaming della risposta.');
                return;
            }

            const reader = res.body.getReader();
            const decoder = new TextDecoder();
            let buffer = '';

            while (true) {
                const { value, done } = await reader.read();
                if (done) break;

                buffer += decoder.decode(value, { stream: true });
                const lines = buffer.split('\n');
                buffer = lines.pop() ?? '';

                for (const line of lines) {
                    if (!line.startsWith('data: ')) continue;
                    let payload;
                    try {
                        payload = JSON.parse(line.slice(6));
                    } catch {
                        continue;
                    }

                    if (payload.token) {
                        accumulated += payload.token;
                        bubble.textContent = accumulated;
                        scrollToBottom();
                    } else if (payload.error) {
                        showError(payload.error);
                    } else if (payload.done) {
                        conversationId = payload.conversation_id ?? conversationId;
                        bubble.textContent = payload.answer ?? accumulated;
                        renderSources(extra, payload.sources);
                        const note = document.createElement('div');
                        note.className = 'text-muted small mt-1';
                        note.textContent = 'Modalità streaming: feedback non disponibile per questo messaggio.';
                        extra.appendChild(note);
                    }
                }
            }
        } catch {
            showError('Connessione interrotta durante lo streaming.');
        } finally {
            scrollToBottom();
        }
    }

    form.addEventListener('submit', (e) => {
        e.preventDefault();
        const question = input.value.trim();
        if (!question) return;

        appendUserMessage(question);
        input.value = '';
        sendBtn.disabled = true;

        const collectionId = collectionSelect.value || null;
        const task = streamingToggle.checked ? sendStream(question, collectionId) : sendQuery(question, collectionId);
        task.finally(() => {
            sendBtn.disabled = false;
            input.focus();
        });
    });

    input.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            form.requestSubmit();
        }
    });
})();
