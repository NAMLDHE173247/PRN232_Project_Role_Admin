using EbayClone.API.DTOs.Disputes;
using EbayClone.API.Models;

namespace EbayClone.API.Repositories;

public interface IDisputeRepository
{
    Task<(int Total, IReadOnlyList<DisputeDto> Items)> GetPageAsync(
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<DisputeDto?> GetDtoByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Dispute?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
