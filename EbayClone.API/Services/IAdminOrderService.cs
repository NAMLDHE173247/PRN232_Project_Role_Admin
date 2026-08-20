using EbayClone.API.DTOs.Orders;

namespace EbayClone.API.Services;

public interface IAdminOrderService
{
    Task<PagedOrderResultDto> GetOrdersAsync(
        string? status,
        DateTime? from,
        DateTime? to,
        int? buyerId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<OrderDetailAdminDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
}
