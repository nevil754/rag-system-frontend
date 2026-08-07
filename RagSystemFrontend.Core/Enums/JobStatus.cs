namespace RagSystemFrontend.Core.Enums;

/// <summary>Stato di un ingestion_jobs (pipeline di ingestione documenti).</summary>
public enum JobStatus
{
    Queued,
    Running,
    Done,
    Failed,
    Cancelled,
}
