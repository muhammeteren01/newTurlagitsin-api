using Core.DTOs.Reservation;
using Core.Entities;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;
using System.Text.Json;
using Core.DTOs.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace Service.Service
{
    public class ReservationService : Service<Reservation>, IReservationService
    {
        private readonly ITripRepository _tripRepository;

        public ReservationService(
            IGenericRepository<Reservation> repository, 
            IUnitOfWork unitOfWork,
            ITripRepository tripRepository) 
            : base(repository, unitOfWork)
        {
            _tripRepository = tripRepository;
        }

        public async Task<List<ReservationResponseDto>> GetAllReservationsAsync()
        {
            var reservations = await _repository.Where(r => true)
                .Include(r => r.User)
                .Include(r => r.Trip)
                .ToListAsync();

            return reservations.Select(MapToResponseDto).ToList();
        }

        public async Task<ReservationResponseDto?> GetReservationByIdAsync(Guid id)
        {
            var reservation = await _repository.Where(r => r.Id == id)
                .Include(r => r.User)
                .Include(r => r.Trip)
                .FirstOrDefaultAsync();

            return reservation == null ? null : MapToResponseDto(reservation);
        }

        private ReservationResponseDto MapToResponseDto(Reservation reservation)
        {
            var seatNumbers = new List<int>();
            if (!string.IsNullOrEmpty(reservation.SeatNumbers))
            {
                try
                {
                    seatNumbers = JsonSerializer.Deserialize<List<int>>(reservation.SeatNumbers) ?? new();
                }
                catch
                {
                    seatNumbers = new List<int>();
                }
            }

            return new ReservationResponseDto
            {
                Id = reservation.Id.ToString(),
                UserId = reservation.UserId.ToString(),
                TripId = reservation.TripId.ToString(),
                CompanyId = reservation.Trip?.CompanyId.ToString() ?? string.Empty,
                SeatNumbers = seatNumbers,
                TotalAmount = reservation.TotalAmount,
                Currency = reservation.Currency ?? "TRY",
                Status = reservation.Status ?? "pending",
                CreatedAt = reservation.CreatedAt,
                UpdatedAt = reservation.UpdatedAt ?? reservation.CreatedAt
            };
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto dto, Guid userId)
        {
            var trip = await _tripRepository.Where(t => t.Id == dto.TripId).FirstOrDefaultAsync();
            if (trip == null)
                throw new Exception("Trip not found");

            var reservation = new Reservation
            {
                UserId = userId,
                TripId = dto.TripId,
                CompanyId = trip.CompanyId,
                SeatNumbers = System.Text.Json.JsonSerializer.Serialize(dto.SeatNumbers),
                TotalAmount = 0, // TODO: Calculate from pricing
                Currency = "TRY",
                Status = "pending",
                Notes = dto.Notes
            };

            await AddAsync(reservation);

            return new ReservationResponseDto
            {
                Id = reservation.Id.ToString(),
                UserId = reservation.UserId.ToString(),
                TripId = reservation.TripId.ToString(),
                SeatNumbers = dto.SeatNumbers,
                TotalAmount = reservation.TotalAmount,
                Currency = reservation.Currency,
                Status = reservation.Status,
                CreatedAt = reservation.CreatedAt,
                UpdatedAt = reservation.CreatedAt
            };
        }

        public async Task<ReservationResponseDto?> UpdateReservationStatusAsync(Guid id, UpdateReservationStatusDto dto)
        {
            var reservation = await _repository.Where(r => r.Id == id).FirstOrDefaultAsync();
            if (reservation == null) return null;

            reservation.Status = dto.Status;
            reservation.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(reservation);
            return await GetReservationByIdAsync(id);
        }

        public async Task<ReservationResponseDto?> ProcessPaymentAsync(ProcessPaymentDto dto)
        {
            var reservation = await _repository.Where(r => r.Id == dto.ReservationId).FirstOrDefaultAsync();
            if (reservation == null) return null;

            reservation.PaymentMethod = dto.PaymentMethod;
            reservation.TransactionId = dto.TransactionId;
            reservation.PaymentDate = DateTime.UtcNow;
            reservation.Status = "completed";
            reservation.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(reservation);
            return await GetReservationByIdAsync(dto.ReservationId);
        }

        public async Task<bool> CancelReservationAsync(Guid id, string reason)
        {
            var reservation = await _repository.Where(r => r.Id == id).FirstOrDefaultAsync();
            if (reservation == null) return false;

            reservation.Status = "cancelled";
            reservation.CancellationDate = DateTime.UtcNow;
            reservation.CancellationReason = reason;
            reservation.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(reservation);
            return true;
        }
    }
}
