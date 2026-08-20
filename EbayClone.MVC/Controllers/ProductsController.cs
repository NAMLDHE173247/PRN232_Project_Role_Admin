using EbayClone.MVC.Filters;
using EbayClone.MVC.Models;
using EbayClone.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.MVC.Controllers;

[AdminSession]
public class ProductsController(AdminApiClient apiClient) : AdminMvcController
{
    public async Task<IActionResult> Index(string? status, int page = 1, CancellationToken cancellationToken = default)
    {
        var query = $"api/admin/products?page={Math.Max(page, 1)}&pageSize=20";
        if (!string.IsNullOrWhiteSpace(status)) query += $"&status={Uri.EscapeDataString(status)}";
        ViewBag.Status = status;
        return View(await apiClient.GetAsync<PagedViewModel<AdminProductViewModel>>(query, cancellationToken));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Hide(int id, CancellationToken cancellationToken) => RunAction(id, "hide", cancellationToken);

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Unhide(int id, CancellationToken cancellationToken) => RunAction(id, "unhide", cancellationToken);

    private async Task<IActionResult> RunAction(int id, string action, CancellationToken cancellationToken)
    {
        try
        {
            await apiClient.PutAsync<AdminProductViewModel>($"api/admin/products/{id}/{action}", null, cancellationToken);
            TempData["Success"] = "Cập nhật sản phẩm thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (AdminApiException exception)
        {
            return HandleApiFailure(exception);
        }
    }
}
