using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.DTOs.Common;
using RagSystemFrontend.Core.ServiceContracts;

namespace RagSystemFrontend.UI.ServicesImplementations;


public class ChatApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    : ApiClientBase(httpClient, httpContextAccessor), IChatApiClient
{
    public Task<ApiResult<ChatQueryResponse>> QueryAsync(ChatQueryRequest request, CancellationToken ct = default) =>
        SendAsync<ChatQueryResponse>(CreateTenantRequest(HttpMethod.Post, "chat/query", request), ct);

    public Task<ApiResult<ChatFeedbackResponse>> SendFeedbackAsync(ChatFeedbackRequest request, CancellationToken ct = default) =>
        SendAsync<ChatFeedbackResponse>(CreateTenantRequest(HttpMethod.Post, "chat/feedback", request), ct);

    public Task<ApiResult<ChatHistoryResponse>> GetHistoryAsync(
        string? conversationId, long? beforeId, int limit, CancellationToken ct = default)
    {
        var query = new List<string> { $"limit={limit}" };
        if (!string.IsNullOrWhiteSpace(conversationId)) query.Add($"conversation_id={Uri.EscapeDataString(conversationId)}");
        if (beforeId.HasValue) query.Add($"before_id={beforeId.Value}");

        return SendAsync<ChatHistoryResponse>(
            CreateTenantRequest(HttpMethod.Get, $"chat/history?{string.Join('&', query)}"), ct);
    }

    public Task<HttpResponseMessage> OpenStreamAsync(ChatQueryRequest request, CancellationToken ct = default)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/stream")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request, RagApiJsonOptions.Default), Encoding.UTF8, "application/json"),
        };
        if (!string.IsNullOrEmpty(TenantToken))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TenantToken);
        }
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        return HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, ct);
    }
}


