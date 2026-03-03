using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs.ResponseDto;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewResponseDto>>> GetAll()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponseDto>> GetById(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewResponseDto>> Create([FromBody] Core.DTOs.Review.CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: Get userId from JWT token
            var userId = Guid.NewGuid();
            var review = await _reviewService.CreateReviewAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // TODO: Get userId from JWT token
            var userId = Guid.NewGuid();
            var deleted = await _reviewService.DeleteReviewAsync(id, userId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
