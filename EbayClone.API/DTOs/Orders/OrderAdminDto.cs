namespace EbayClone.API.DTOs.Orders;

public record OrderAdminDto(
    int OrderId,
    int? BuyerId,
    string? BuyerName,
    string? Status,
    decimal TotalAmount,
    DateTime? OrderDate);
