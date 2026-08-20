using EbayClone.API.DTOs.Audit;
using EbayClone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/audit-logs")]
public class AuditLogController(IAuditService auditService) : ControllerBase
{
    [HttpGet]
    public Task<PagedAuditResultDto> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return auditService.GetPageAsync(page, pageSize, cancellationToken);
    }
}
