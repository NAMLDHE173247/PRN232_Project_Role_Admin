using EbayClone.API.DTOs.Orders;

namespace EbayClone.API.Repositories;

public interface IOrderRepository
{
    Task<(int Total, IReadOnlyList<OrderAdminDto> Items)> GetPageAsync(
        string? status,
        DateTime? from,
        DateTime? to,
        int? buyerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<OrderDetailAdminDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
}
