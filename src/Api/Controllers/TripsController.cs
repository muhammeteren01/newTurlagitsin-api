using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs.ResponseDto;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TripResponseDto>>> GetAll()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripResponseDto>> GetById(Guid id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null)
                return NotFound();

            return Ok(trip);
        }

        [HttpPost]
        public async Task<ActionResult<TripResponseDto>> Create([FromBody] Core.DTOs.Trip.CreateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = await _tripService.CreateTripAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = trip.Id }, trip);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TripResponseDto>> Update(Guid id, [FromBody] Core.DTOs.Trip.UpdateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var trip = await _tripService.UpdateTripAsync(id, dto);
            if (trip == null)
                return NotFound();

            return Ok(trip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _tripService.DeleteTripAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
