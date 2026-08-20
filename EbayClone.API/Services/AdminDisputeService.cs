using System.Text.Json;
using EbayClone.API.DTOs.Disputes;
using EbayClone.API.Models;
using EbayClone.API.Repositories;

namespace EbayClone.API.Services;

public class AdminDisputeService(IDisputeRepository disputeRepository, IAuditRepository auditRepository)
    : IAdminDisputeService
{
    public async Task<PagedDisputeResultDto> GetDisputesAsync(
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await disputeRepository.GetPageAsync(status, page, pageSize, cancellationToken);
        return new PagedDisputeResultDto(page, pageSize, result.Total, result.Items);
    }

    public Task<DisputeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        disputeRepository.GetDtoByIdAsync(id, cancellationToken);

    public async Task<DisputeDto?> AssignAsync(
        int id,
        int actorAdminId,
        int? assignedAdminId,
        CancellationToken cancellationToken = default)
    {
        var dispute = await disputeRepository.GetByIdAsync(id, cancellationToken);
        if (dispute is null) return null;
        if (dispute.Status != nameof(DisputeStatus.Open))
            throw new InvalidOperationException("Only open disputes can be assigned.");

        var targetAdminId = assignedAdminId ?? actorAdminId;
        if (!await disputeRepository.IsAdminAsync(targetAdminId, cancellationToken))
            throw new InvalidOperationException("Assigned user must have the Admin role.");

        dispute.AssignedTo = targetAdminId;
        dispute.AssignedAt = DateTime.UtcNow;
        dispute.Status = nameof(DisputeStatus.Assigned);
        await disputeRepository.SaveChangesAsync(cancellationToken);
        await WriteAuditAsync(actorAdminId, "ASSIGN_DISPUTE", dispute, new { assignedTo = targetAdminId }, cancellationToken);
        return await disputeRepository.GetDtoByIdAsync(id, cancellationToken);
    }

    public Task<DisputeDto?> ResolveAsync(
        int id,
        int adminId,
        string resolution,
        CancellationToken cancellationToken = default) =>
        FinishAsync(id, adminId, resolution, DisputeStatus.Assigned, DisputeStatus.Resolved, "RESOLVE_DISPUTE", cancellationToken);

    public Task<DisputeDto?> RejectAsync(
        int id,
        int adminId,
        string resolution,
        CancellationToken cancellationToken = default) =>
        FinishAsync(id, adminId, resolution, DisputeStatus.Open, DisputeStatus.Rejected, "REJECT_DISPUTE", cancellationToken);

    private async Task<DisputeDto?> FinishAsync(
        int id,
        int adminId,
        string resolution,
        DisputeStatus expectedStatus,
        DisputeStatus nextStatus,
        string auditAction,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resolution))
            throw new InvalidOperationException("Resolution is required.");
        if (resolution.Trim().Length > 2000)
            throw new InvalidOperationException("Resolution cannot exceed 2000 characters.");

        var dispute = await disputeRepository.GetByIdAsync(id, cancellationToken);
        if (dispute is null) return null;
        if (dispute.Status != expectedStatus.ToString())
            throw new InvalidOperationException($"Only {expectedStatus} disputes can be changed by this action.");

        dispute.Status = nextStatus.ToString();
        dispute.Resolution = resolution.Trim();
        dispute.ResolvedBy = adminId;
        dispute.ResolvedAt = DateTime.UtcNow;
        await disputeRepository.SaveChangesAsync(cancellationToken);
        await WriteAuditAsync(adminId, auditAction, dispute, new { status = dispute.Status }, cancellationToken);
        return await disputeRepository.GetDtoByIdAsync(id, cancellationToken);
    }

    private Task WriteAuditAsync(
        int adminId,
        string action,
        Dispute dispute,
        object metadata,
        CancellationToken cancellationToken)
    {
        return auditRepository.AddAsync(new AuditLog
        {
            ActorId = adminId,
            Action = action,
            Resource = "DISPUTE",
            ResourceId = dispute.Id,
            Metadata = JsonSerializer.Serialize(metadata),
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);
    }
}
