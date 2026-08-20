using EbayClone.API.Models;

namespace EbayClone.API.Repositories;

public interface IProductRepository
{
    Task<(int Total, IReadOnlyList<Product> Items)> GetPageAsync(
        ProductStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
