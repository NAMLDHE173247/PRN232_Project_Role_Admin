using EbayClone.API.Models;

namespace EbayClone.API.DTOs.Products;

public record AdminProductDto(
    int Id,
    string Name,
    decimal Price,
    int SellerId,
    ProductStatus Status);
