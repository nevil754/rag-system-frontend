(() => {
    const pollableStatuses = ['pending', 'processing'];
    const terminalJobStatuses = { done: 'ready', failed: 'error', cancelled: 'error' };

    let active = Array.from(document.querySelectorAll('#documentsTable tr[data-document-id]'))
        .filter((row) => pollableStatuses.includes(row.dataset.status));

    if (active.length === 0) return;

    function badgeClassFor(status) {
        switch (status) {
            case 'ready': return 'text-bg-success';
            case 'error': return 'text-bg-danger';
            case 'deleted': return 'text-bg-secondary';
            default: return 'text-bg-warning';
        }
    }

    async function pollRow(row) {
        const id = row.dataset.documentId;
        try {
            const res = await fetch(`/Documents/Status/${id}`);
            if (!res.ok) return false;

            const job = await res.json();
            const mapped = terminalJobStatuses[job.status];
            if (!mapped) return false;

            row.dataset.status = mapped;
            const badge = row.querySelector('.status-badge');
            if (badge) {
                badge.textContent = mapped;
                badge.className = 'badge status-badge ' + badgeClassFor(mapped);
            }
            return true;
        } catch {
            return false;
        }
    }

    const interval = setInterval(async () => {
        const results = await Promise.all(active.map(pollRow));
        active = active.filter((_, i) => !results[i]);

        if (active.length === 0) {
            clearInterval(interval);
            // ricarica per mostrare chunk_count/page_count aggiornati (non presenti nello stato job)
            window.location.reload();
        }
    }, 3000);
})();
