namespace Core.DTOs.Reservation
{
    public class CreateReservationDto
    {
        public Guid TripId { get; set; }
        public List<int> SeatNumbers { get; set; } = new();
        public string? Notes { get; set; }
    }
}