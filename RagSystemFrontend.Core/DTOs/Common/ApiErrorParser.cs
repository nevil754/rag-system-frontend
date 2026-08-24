using System.Text.Json;

namespace RagSystemFrontend.Core.DTOs.Common;

/// <summary>
/// Interpreta i corpi di errore del backend: il formato standard FastAPI {"detail": ...}
/// (stringa o lista di errori di validazione Pydantic), e il formato speciale del rate
/// limiter {"error","detail","retry_after"} (vedi README4.md §10).
/// </summary>
public static class ApiErrorParser
{
    public static (string Message, int? RetryAfterSeconds) Parse(int statusCode, string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return (DefaultMessageFor(statusCode), null);
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errorProp) && root.TryGetProperty("detail", out var rlDetail)
                && errorProp.ValueKind == JsonValueKind.String && rlDetail.ValueKind == JsonValueKind.String)
            {
                int? retryAfter = root.TryGetProperty("retry_after", out var ra) && ra.TryGetInt32(out var raVal)
                    ? raVal
                    : null;
                var message = $"{errorProp.GetString()}: {rlDetail.GetString()}";
                return (message, retryAfter);
            }

            if (root.TryGetProperty("detail", out var detailProp))
            {
                if (detailProp.ValueKind == JsonValueKind.String)
                {
                    return (detailProp.GetString() ?? DefaultMessageFor(statusCode), null);
                }

                if (detailProp.ValueKind == JsonValueKind.Array)
                {
                    var messages = new List<string>();
                    foreach (var item in detailProp.EnumerateArray())
                    {
                        if (item.TryGetProperty("msg", out var msg) && msg.ValueKind == JsonValueKind.String)
                        {
                            var loc = item.TryGetProperty("loc", out var locProp)
                                ? string.Join(".", locProp.EnumerateArray().Select(l => l.ToString()))
                                : null;
                            messages.Add(string.IsNullOrEmpty(loc) ? msg.GetString() ?? "" : $"{loc}: {msg.GetString()}");
                        }
                    }
                    return (messages.Count > 0 ? string.Join("; ", messages) : DefaultMessageFor(statusCode), null);
                }
            }
        }
        catch (Exception)
        {
            // corpo JSON con forma inattesa, o non-JSON (es. errore HTML/testo grezzo di un proxy) - fallback sotto.
            // Questo parser non deve mai propagare eccezioni: è pensato per degradare sempre a un messaggio generico.
        }

        return (DefaultMessageFor(statusCode), null);
    }

    private static string DefaultMessageFor(int statusCode) => statusCode switch
    {
        400 => "Richiesta non valida.",
        401 => "Sessione scaduta o non autenticata. Effettua di nuovo l'accesso.",
        403 => "Non hai i permessi necessari per questa operazione.",
        404 => "Risorsa non trovata.",
        409 => "Conflitto: la risorsa esiste già o non è in uno stato valido.",
        422 => "Dati non validi.",
        429 => "Troppe richieste. Riprova tra qualche istante.",
        >= 500 => "Errore del server. Riprova più tardi.",
        _ => "Si è verificato un errore imprevisto.",
    };
}
