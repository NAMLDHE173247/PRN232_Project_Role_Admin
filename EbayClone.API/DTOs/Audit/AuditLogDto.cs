namespace EbayClone.API.DTOs.Audit;

public record AuditLogDto(
    long Id,
    int? ActorId,
    string Action,
    string Resource,
    int? ResourceId,
    string? Metadata,
    DateTime CreatedAtUtc);
