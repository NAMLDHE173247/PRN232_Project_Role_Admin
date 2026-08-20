using EbayClone.MVC.Filters;
using EbayClone.MVC.Models;
using EbayClone.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.MVC.Controllers;

[AdminSession]
public class FeedbacksController(AdminApiClient apiClient) : AdminMvcController
{
    public async Task<IActionResult> Index(
        int? sellerId,
        decimal? minRating,
        decimal? maxRating,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var query = $"api/admin/feedbacks?page={Math.Max(page, 1)}&pageSize=20";
        if (sellerId.HasValue) query += $"&sellerId={sellerId.Value}";
        if (minRating.HasValue) query += $"&minRating={minRating.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        if (maxRating.HasValue) query += $"&maxRating={maxRating.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

        ViewBag.SellerId = sellerId;
        ViewBag.MinRating = minRating;
        ViewBag.MaxRating = maxRating;
        return View(await apiClient.GetAsync<PagedViewModel<AdminFeedbackViewModel>>(query, cancellationToken));
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var feedback = await apiClient.GetAsync<AdminFeedbackViewModel>($"api/admin/feedbacks/{id}", cancellationToken);
        return feedback is null ? NotFound() : View(feedback);
    }
}
