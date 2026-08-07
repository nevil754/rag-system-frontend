namespace RagSystemFrontend.Core.Enums;

/// <summary>Ruoli utente a livello tenant ([schema_tenant].users.role, vincolato da CHECK SQL).</summary>
public enum UserRole
{
    Admin,
    User,
    Viewer,
}
