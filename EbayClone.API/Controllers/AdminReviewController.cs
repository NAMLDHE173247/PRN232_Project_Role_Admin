using System.Security.Claims;
using EbayClone.API.DTOs.Reviews;
using EbayClone.API.Models;
using EbayClone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.API.Controllers;

/// <summary>Moderation endpoints for product reviews.</summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/reviews")]
public class AdminReviewController(IAdminReviewService reviewService) : ControllerBase
{
    [HttpGet]
    public Task<PagedReviewResultDto> GetReviews(
        [FromQuery] ReviewStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default) =>
        reviewService.GetReviewsAsync(status, page, pageSize, cancellationToken);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminReviewDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var review = await reviewService.GetByIdAsync(id, cancellationToken);
        return review is null ? NotFound() : Ok(review);
    }

    [HttpPut("{id:int}/hide")]
    public Task<ActionResult<AdminReviewDto>> Hide(int id, CancellationToken cancellationToken) =>
        ExecuteTransition(() => reviewService.HideAsync(id, GetAdminId(), cancellationToken));

    [HttpPut("{id:int}/unhide")]
    public Task<ActionResult<AdminReviewDto>> Unhide(int id, CancellationToken cancellationToken) =>
        ExecuteTransition(() => reviewService.UnhideAsync(id, GetAdminId(), cancellationToken));

    private static async Task<ActionResult<AdminReviewDto>> ExecuteTransition(Func<Task<AdminReviewDto?>> transition)
    {
        try
        {
            var review = await transition();
            return review is null ? new NotFoundResult() : new OkObjectResult(review);
        }
        catch (InvalidOperationException exception)
        {
            return new BadRequestObjectResult(new { message = exception.Message });
        }
    }

    private int GetAdminId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var adminId)
            ? adminId
            : throw new InvalidOperationException("Authenticated user id is missing.");
    }
}
