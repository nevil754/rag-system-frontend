namespace RagSystemFrontend.Core.Enums;

/// <summary>Converte gli enum nel valore stringa lowercase atteso dal backend (es. Admin -> "admin").</summary>
public static class EnumApiExtensions
{
    public static string ToApiValue(this Enum value) => value.ToString().ToLowerInvariant();
}
