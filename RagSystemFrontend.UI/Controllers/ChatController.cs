using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using RagSystemFrontend.Core.DTOs;
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Models.ViewModels;

namespace RagSystemFrontend.UI.Controllers;


[Authorize(Policy = "TenantAuth")]
public class ChatController(
    IChatApiClient chatApiClient,
    ICollectionsApiClient collectionsApiClient,
    ILogger<ChatController> logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        var collectionsResult = await collectionsApiClient.GetCollectionsAsync(1, 100);
        if (!collectionsResult.Success && collectionsResult.IsUnauthorized)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account", new { expired = true });
        }
        return View(new ChatIndexViewModel
        {
            Collections = collectionsResult.Success ? collectionsResult.Data?.Items ?? [] : [],
        });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send([FromBody] ChatSendViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Question))
        {
            return BadRequest(new { error = "La domanda non può essere vuota." });
        }
        var result = await chatApiClient.QueryAsync(new ChatQueryRequest
        {
            Question = model.Question,
            ConversationId = string.IsNullOrWhiteSpace(model.ConversationId) ? null : model.ConversationId,
            CollectionId = string.IsNullOrWhiteSpace(model.CollectionId) ? null : model.CollectionId,
        }, ct);
        if (!result.Success || result.Data is null)
        {
            if (result.IsUnauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Unauthorized(new { error = result.ErrorMessage, unauthorized = true });
            }

            return StatusCode(result.StatusCode == 0 ? 502 : result.StatusCode, new { error = result.ErrorMessage });
        }
        return Json(result.Data);
    }


    [HttpGet]
    public async Task<IActionResult> History(string? conversationId, long? beforeId, int limit = 20, CancellationToken ct = default)
    {
        var result = await chatApiClient.GetHistoryAsync(conversationId, beforeId, limit, ct);
        if (!result.Success)
        {
            if (result.IsUnauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Unauthorized(new { error = result.ErrorMessage, unauthorized = true });
            }

            return StatusCode(result.StatusCode == 0 ? 502 : result.StatusCode, new { error = result.ErrorMessage });
        }
        return Json(result.Data);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Feedback([FromBody] ChatFeedbackViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Feedback non valido." });
        }
        var result = await chatApiClient.SendFeedbackAsync(new ChatFeedbackRequest
        {
            MessageId = model.MessageId,
            Rating = model.Rating,
            Comment = model.Comment,
        }, ct);

        if (!result.Success || result.Data is null)
        {
            if (result.IsUnauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Unauthorized(new { error = result.ErrorMessage, unauthorized = true });
            }

            return StatusCode(result.StatusCode == 0 ? 502 : result.StatusCode, new { error = result.ErrorMessage });
        }
        return Json(result.Data);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task Stream([FromBody] ChatSendViewModel model, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(model.Question))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        using var upstream = await chatApiClient.OpenStreamAsync(new ChatQueryRequest
        {
            Question = model.Question,
            ConversationId = string.IsNullOrWhiteSpace(model.ConversationId) ? null : model.ConversationId,
            CollectionId = string.IsNullOrWhiteSpace(model.CollectionId) ? null : model.CollectionId,
        }, ct);
        if (!upstream.IsSuccessStatusCode)
        {
            if (upstream.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            Response.StatusCode = (int)upstream.StatusCode;
            return;
        }
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Append("X-Accel-Buffering", "no");
        HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
        try
        {
            await using var upstreamStream = await upstream.Content.ReadAsStreamAsync(ct);
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = await upstreamStream.ReadAsync(buffer, ct)) > 0)
            {
                await Response.Body.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                await Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException)
        {
            // client ha chiuso la connessione (navigazione via, stop manuale): niente da fare
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Errore durante il proxy dello streaming chat");
        }
    }


}
