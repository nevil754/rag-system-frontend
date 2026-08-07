using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;

namespace RagSystemFrontend.Core.ServiceContracts;

/// <summary>POST /chat/query, /chat/stream, /chat/feedback. Auth: Bearer tenant o X-API-Key.</summary>
public interface IChatApiClient
{
    Task<ApiResult<ChatQueryResponse>> QueryAsync(ChatQueryRequest request, CancellationToken ct = default);

    Task<ApiResult<ChatFeedbackResponse>> SendFeedbackAsync(ChatFeedbackRequest request, CancellationToken ct = default);

    /// <summary>
    /// Apre la connessione SSE grezza verso /chat/stream con HttpCompletionOption.ResponseHeadersRead,
    /// per essere inoltrata byte-per-byte al browser da un controller proxy. Il chiamante è
    /// responsabile di leggere/chiudere lo stream della risposta.
    /// </summary>
    Task<HttpResponseMessage> OpenStreamAsync(ChatQueryRequest request, CancellationToken ct = default);
}
