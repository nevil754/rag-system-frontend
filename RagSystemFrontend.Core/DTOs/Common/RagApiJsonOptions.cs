using System.Text.Json;

namespace RagSystemFrontend.Core.DTOs.Common;

/// <summary>
/// Opzioni JSON condivise da tutti i client API: il backend FastAPI serializza in snake_case.
/// </summary>
public static class RagApiJsonOptions
{
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };
}
