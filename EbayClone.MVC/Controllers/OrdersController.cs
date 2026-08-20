using EbayClone.MVC.Filters;
using EbayClone.MVC.Models;
using EbayClone.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.MVC.Controllers;

[AdminSession]
public class OrdersController(AdminApiClient apiClient) : AdminMvcController
{
    public async Task<IActionResult> Index(string? status, int? buyerId, int page = 1, CancellationToken cancellationToken = default)
    {
        var query = $"api/admin/orders?page={Math.Max(page, 1)}&pageSize=20";
        if (!string.IsNullOrWhiteSpace(status)) query += $"&status={Uri.EscapeDataString(status)}";
        if (buyerId.HasValue) query += $"&buyerId={buyerId.Value}";
        ViewBag.Status = status;
        ViewBag.BuyerId = buyerId;
        return View(await apiClient.GetAsync<PagedViewModel<OrderViewModel>>(query, cancellationToken));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var model = await apiClient.GetAsync<OrderDetailViewModel>($"api/admin/orders/{id}", cancellationToken);
        return View(model);
    }
}
