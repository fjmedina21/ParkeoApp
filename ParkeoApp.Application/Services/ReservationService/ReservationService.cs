using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Application.Services.ReservationStateService;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Domain.Enums;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ReservationService
{
	public class ReservationService(ParkeoAppContext dbContext, IMapper mapper, IConfiguration configuration, IReservationStateService stateService) : IReservationService
	{
		private IQueryable<Reservation> LoadData(Guid tenantId) => dbContext.Reservations
			.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(tenantId))
			.Include(e => e.Payments.OrderByDescending(e => e.CreatedAt))
			.Include(e => e.User)
			.Include(e => e.Spot).ThenInclude(e => e.ParkingLot)
			.OrderByDescending(e => e.UpdatedAt).ThenByDescending(e => e.CreatedAt)
			.AsQueryable();

		public async Task<ApiResponse<GetReservation>> ReserveAsync(AddReservation reservation, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Reservation newReservation = mapper.Map<Reservation>(reservation);

			(bool isValid, string? msj) = await CheckReservationAvailabilityAsync(newReservation, jwt);
			if (!isValid) return new ApiResponse<GetReservation>(StatusCodes.Status400BadRequest, message: msj);

			ParkingSpot? spot = await dbContext.ParkingSpots.Include(e => e.ParkingLot)
				.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(Utils.DecodeJwt(jwt).Tenant))
				.FirstOrDefaultAsync(e => e.SpotId.Equals(reservation.SpotId));

			decimal hourRate = spot!.ParkingLot.HourlyRate;
			TimeSpan duration = newReservation.EndAt - newReservation.StartAt;
			var totalHours = (decimal)Math.Ceiling(duration.TotalHours); // Contar horas totales, redondeando hacia arriba (cada fracción cuenta como una hora completa)

			newReservation.TenantId = tokenPayload.Tenant;
			newReservation.UserId = tokenPayload.User;
			newReservation.SpotId = reservation.SpotId;
			newReservation.TotalCost = hourRate * totalHours;

			Random rnd = new();
			var randomNumberInRange = rnd.Next(10000, 99999).ToString();
			newReservation.Code = $"RSV-{randomNumberInRange}";
			var entry = await dbContext.Reservations.AddAsync(newReservation);
			await dbContext.SaveChangesAsync();

			Reservation? createdReservation = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.ReservationId.Equals(entry.Entity.ReservationId));
			await Utils.SendReservationEmailNotification(createdReservation!,
				"creada",
				"ParkeoApp: Reserva creada exitosamente",
				configuration);

			GetReservation? dto = mapper.Map<GetReservation>(createdReservation);
			return new ApiResponse<GetReservation>(StatusCodes.Status201Created, data: [dto]);
		}

		public async Task<ApiResponse<GetReservation>> CheckOutAsync(string reservationCode, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Reservation? entity = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.Code == reservationCode);
			if (entity is null) return new ApiResponse<GetReservation>(statusCode: StatusCodes.Status400BadRequest);

			(bool validTransition, string? msj) = await stateService.ReservationStateTransition(dbContext, entity, ReservationStatus.Completed);
			return !validTransition
				? new ApiResponse<GetReservation>(statusCode: StatusCodes.Status400BadRequest, message: msj)
				: new ApiResponse<GetReservation>(statusCode: StatusCodes.Status200OK, message: "Reservation Checked Out.");
		}

		public async Task<ApiResponse<GetReservation>> CheckInAsync(string reservationCode, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Reservation? entity = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.Code == reservationCode);
			if (entity is null) return new ApiResponse<GetReservation>(statusCode: StatusCodes.Status400BadRequest);

			(bool validTransition, string? msj) = await stateService.ReservationStateTransition(dbContext, entity, ReservationStatus.Active);
			return !validTransition
				? new ApiResponse<GetReservation>(statusCode: StatusCodes.Status400BadRequest, message: msj)
				: new ApiResponse<GetReservation>(statusCode: StatusCodes.Status200OK, message: "Reservation Checked In.");
		}

		public async Task<ApiResponse<GetReservation>> CancelAsync(string reservationCode, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Reservation? entity = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.Code == reservationCode);
			if (entity is null) return new ApiResponse<GetReservation>(statusCode: StatusCodes.Status400BadRequest);

			(bool validTransition, string? msj) = await stateService.ReservationStateTransition(dbContext, entity, ReservationStatus.Cancelled);
			return !validTransition
				? new ApiResponse<GetReservation>(statusCode: StatusCodes.Status400BadRequest, message: msj)
				: new ApiResponse<GetReservation>(statusCode: StatusCodes.Status200OK, message: "Reservation Cancelled.");
		}

		public async Task<ApiResponse<GetReservation>> GetByCodeAsync(string reservationCode, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Reservation? entity = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.Code == reservationCode);
			if (entity is null) return new ApiResponse<GetReservation>(statusCode: StatusCodes.Status404NotFound);
			GetReservation dto = mapper.Map<GetReservation>(entity);
			return new ApiResponse<GetReservation>(statusCode: StatusCodes.Status200OK, data: [dto]);
		}

		public async Task<ApiResponse<GetReservation>> GetByUserAsync(PaginationParams paginationParams, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			var entity = await LoadData(tokenPayload.Tenant).Where(e => e.UserId == tokenPayload.User).ToListAsync();
			var dto = mapper.Map<ICollection<GetReservation>>(entity);

			var pagedItem = PagedList<GetReservation>.ToPagedList(dto, paginationParams.CurrentPage, paginationParams.PageSize);
			return new ApiResponse<GetReservation>(statusCode: StatusCodes.Status200OK, data: pagedItem);
		}

		private async Task<(bool isValid, string? message)> CheckReservationAvailabilityAsync(Reservation reservation, string jwt)
		{
			// 1. Validar fechas
			if (reservation.StartAt <= DateTime.UtcNow.ToLocalTime()) return (false, "The start date cannot be in the past.");
			if (reservation.EndAt <= reservation.StartAt) return (false, "The end date cannot be before the start date.");

			// 2. Validar disponibilidad del espacio
			ParkingSpot? spot = await dbContext.ParkingSpots
				.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(Utils.DecodeJwt(jwt).Tenant))
				.FirstOrDefaultAsync(e => e.SpotId == reservation.SpotId);

			if (spot != null && !spot.Status.Equals(nameof(SpotStatus.Available), StringComparison.CurrentCultureIgnoreCase))
				return (false, "Spot is not available.");

			// 3. Validar conflicto de horario (interpolación)
			bool hasConflict = await LoadData(Utils.DecodeJwt(jwt).Tenant).AnyAsync(r =>
				r.SpotId == reservation.SpotId
			 && r.ReservationId != reservation.ReservationId // evitar compararse con sí misma
			 && r.Status != nameof(ReservationStatus.Cancelled)
			 && r.Status != nameof(ReservationStatus.Completed)
			 && (
					(reservation.StartAt >= r.StartAt && reservation.StartAt < r.EndAt) // empieza dentro de otra
				 || (reservation.EndAt > r.StartAt && reservation.EndAt <= r.EndAt) // termina dentro de otra
				 || (reservation.StartAt <= r.StartAt && reservation.EndAt >= r.EndAt) // la abarca completamente
				)
			);

			return hasConflict
				? (false, "There is a conflict with another reservation.")
				: (true, null);
		}
	}
}
