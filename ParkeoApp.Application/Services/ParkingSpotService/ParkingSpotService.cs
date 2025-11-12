using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Domain.Enums;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ParkingSpotService
{
	public class ParkingSpotService(ParkeoAppContext dbContext, IMapper mapper) : IParkingSpotService
	{
		private IQueryable<ParkingSpot> LoadData(Guid tenantId) => dbContext.ParkingSpots
			.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(tenantId))
			.Include(e => e.ParkingLot)
			.Include(e => e.Reservations.OrderByDescending(e => e.CreatedAt))
			.OrderByDescending(e => e.UpdatedAt).ThenByDescending(e => e.CreatedAt)
			.AsQueryable();

		public async Task<ApiResponse<GetParkingSpotWnRef>> GetByParkingLotAsync(Guid parkingLotId, PaginationParams paginationParams, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			var data = await LoadData(tokenPayload.Tenant).Where(e => e.ParkingLotId.Equals(parkingLotId)).ToListAsync();
			var dto = mapper.Map<ICollection<GetParkingSpotWnRef>>(data);

			var pagedItem = PagedList<GetParkingSpotWnRef>.ToPagedList(dto, paginationParams.CurrentPage, paginationParams.PageSize);
			return new ApiResponse<GetParkingSpotWnRef>(data: pagedItem);
		}
		public async Task<ApiResponse> MarkSpotAsMaintenanceAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingSpot? spot = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.SpotId.Equals(uid));
			if (spot is null) return new ApiResponse(StatusCodes.Status400BadRequest);

			(bool validForMaintenance, string? errMsj) =await ValidateParkingSpotForMaintenance(spot, jwt);
			if(!validForMaintenance) return new ApiResponse(StatusCodes.Status400BadRequest, errMsj);

			spot.Status = nameof(SpotStatus.Maintenance);
			await dbContext.SaveChangesAsync();

			return new ApiResponse(message: "The parking spot has been marked as under maintenance.");
		}

		public async Task<ApiResponse> MarkSpotAsAvailableAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingSpot? spot = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.SpotId.Equals(uid));
			if (spot is null) return new ApiResponse(StatusCodes.Status400BadRequest);

			if(!spot.Status.Equals(nameof(SpotStatus.Maintenance))) return new ApiResponse(StatusCodes.Status400BadRequest, message:"The parking spot is not under maintenance.");

			spot.Status = nameof(SpotStatus.Available);
			await dbContext.SaveChangesAsync();

			return new ApiResponse(message: "The parking spot has been marked as available.");
		}

		private async Task<(bool, string?)> ValidateParkingSpotForMaintenance(ParkingSpot spot, string jwt)
		{
			switch (spot.Status)
			{
				// Verifica si el spot está actualmente ocupado o reservado
				case nameof(SpotStatus.Occupied):
					return (false, "The parking spot is currently occupied and cannot be set to maintenance.");
				case nameof(SpotStatus.Maintenance):
					return (false, "The parking spot is already under maintenance.");
				default:
				{
					// Asegura que no haya reservas activas o en curso
					bool hasActiveReservations = await LoadData(Utils.DecodeJwt(jwt).Tenant).AnyAsync(r =>
						r.SpotId == spot.SpotId && (r.Status == nameof(ReservationStatus.Reserved) || r.Status == nameof(ReservationStatus.Active)));

					return (!hasActiveReservations) ? (true, null) : (false, "The parking spot has active or pending reservations and cannot be set to maintenance.");
				}
			}
		}

		public Task<ApiResponse<GetParkingSpotWnRef>> GetAllAsync(PaginationParams paginationParams, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse<GetParkingSpotWnRef>> GetByIdAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingSpot? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.SpotId.Equals(uid));
			if (data is null) return new ApiResponse<GetParkingSpotWnRef>(StatusCodes.Status400BadRequest);
			GetParkingSpotWnRef? dto = mapper.Map<GetParkingSpotWnRef>(data);
			return new ApiResponse<GetParkingSpotWnRef>(data: [dto]);
		}

		public async Task<ApiResponse<GetParkingSpotWnRef>> CreateAsync(AddParkingSpot model, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingSpot newParkingSpot = mapper.Map<ParkingSpot>(model);

			newParkingSpot.TenantId = tokenPayload.Tenant;
			var entry = await dbContext.ParkingSpots.AddAsync(newParkingSpot);
			await dbContext.SaveChangesAsync();

			GetParkingSpotWnRef? dto = mapper.Map<GetParkingSpotWnRef>(entry.Entity);
			return new ApiResponse<GetParkingSpotWnRef>(StatusCodes.Status201Created, data: [dto]);
		}

		public Task<ApiResponse> UpdateAsync(Guid uid, AddParkingSpot model, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse> DeleteAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingSpot? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.SpotId.Equals(uid));
			if (data is null) return new ApiResponse(StatusCodes.Status400BadRequest);
			data.DeletedAt = DateTime.UtcNow;
			return new ApiResponse(StatusCodes.Status204NoContent);
		}
	}
}
