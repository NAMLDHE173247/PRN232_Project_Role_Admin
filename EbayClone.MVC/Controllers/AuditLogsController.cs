using EbayClone.MVC.Filters;
using EbayClone.MVC.Models;
using EbayClone.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.MVC.Controllers;

[AdminSession]
public class AuditLogsController(AdminApiClient apiClient) : AdminMvcController
{
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        var model = await apiClient.GetAsync<PagedViewModel<AuditLogViewModel>>(
            $"api/admin/audit-logs?page={Math.Max(page, 1)}&pageSize=20",
            cancellationToken);
        return View(model);
    }
}
