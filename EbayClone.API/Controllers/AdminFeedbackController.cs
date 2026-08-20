using EbayClone.API.DTOs.Feedbacks;
using EbayClone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.API.Controllers;

/// <summary>Read-only monitoring of seller feedback aggregates.</summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/feedbacks")]
public class AdminFeedbackController(IAdminFeedbackService feedbackService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedFeedbackResultDto>> GetFeedbacks(
        [FromQuery] int? sellerId,
        [FromQuery] decimal? minRating,
        [FromQuery] decimal? maxRating,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await feedbackService.GetFeedbacksAsync(
                sellerId, minRating, maxRating, page, pageSize, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminFeedbackDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var feedback = await feedbackService.GetByIdAsync(id, cancellationToken);
        return feedback is null ? NotFound() : Ok(feedback);
    }
}
