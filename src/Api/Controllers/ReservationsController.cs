using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs.ResponseDto;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReservationResponseDto>>> GetAll()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationResponseDto>> GetById(Guid id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationResponseDto>> Create([FromBody] Core.DTOs.Reservation.CreateReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO: Get userId from JWT token
            var userId = Guid.NewGuid(); 
            var reservation = await _reservationService.CreateReservationAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ReservationResponseDto>> UpdateStatus(Guid id, [FromBody] Core.DTOs.Reservation.UpdateReservationStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = await _reservationService.UpdateReservationStatusAsync(id, dto);
            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        [HttpPost("payment")]
        public async Task<ActionResult<ReservationResponseDto>> ProcessPayment([FromBody] Core.DTOs.Reservation.ProcessPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = await _reservationService.ProcessPaymentAsync(dto);
            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(Guid id, [FromQuery] string reason = "User cancelled")
        {
            var cancelled = await _reservationService.CancelReservationAsync(id, reason);
            if (!cancelled)
                return NotFound();

            return NoContent();
        }
    }
}
