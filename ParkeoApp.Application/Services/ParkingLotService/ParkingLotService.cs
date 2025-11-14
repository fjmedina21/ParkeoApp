using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Domain.Enums;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ParkingLotService
{
	public class ParkingLotService(ParkeoAppContext dbContext, IMapper mapper, IConfiguration configuration) : IParkingLotService
	{
		private IQueryable<ParkingLot> LoadData(Guid tenantId) => dbContext.ParkingLots
			.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(tenantId))
			.Include(e => e.ParkingSpots.OrderByDescending(spot => spot.CreatedAt))
				.ThenInclude(e => e.Reservations.OrderByDescending(reservation => reservation.CreatedAt))
			.OrderByDescending(e => e.UpdatedAt).ThenByDescending(e => e.CreatedAt)
			.AsQueryable();

		public async Task<ApiResponse<GetParkingLot>> GetAllAsync(PaginationParams paginationParams, string jwt, double? originLat, double? originLng)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			var data = await LoadData(tokenPayload.Tenant).ToListAsync();
			var dto = mapper.Map<List<GetParkingLot>>(data);

			// Si se reciben coordenadas, ordenamos por distancia usando Routes API
			if (originLat.HasValue && originLng.HasValue) dto = await GoogleUtils.GetLotsOrderedByDistanceAsync(originLat.Value, originLng.Value, dto, configuration);

			var pagedItem = PagedList<GetParkingLot>.ToPagedList(dto, paginationParams.CurrentPage, paginationParams.PageSize);
			return new ApiResponse<GetParkingLot>(data: pagedItem);
		}

		public Task<ApiResponse<GetParkingLot>> GetAllAsync(PaginationParams paginationParams, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse<GetParkingLot>> GetByIdAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingLot? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.ParkingLotId.Equals(uid));
			if (data is null) return new ApiResponse<GetParkingLot>(StatusCodes.Status400BadRequest);
			GetParkingLot? dto = mapper.Map<GetParkingLot>(data);
			return new ApiResponse<GetParkingLot>(data: [dto]);
		}

		public async Task<ApiResponse<GetParkingLot>> CreateAsync(AddParkingLot model, string jwt)
		{
			// ToDo: validate spot capacity
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingLot newParkingLot = mapper.Map<ParkingLot>(model);

			newParkingLot.TenantId = tokenPayload.Tenant;
			foreach (ParkingSpot spot in newParkingLot.ParkingSpots) spot.TenantId = tokenPayload.Tenant;
			var entry = await dbContext.ParkingLots.AddAsync(newParkingLot);
			await dbContext.SaveChangesAsync();

			GetParkingLot? dto = mapper.Map<GetParkingLot>(entry.Entity);
			return new ApiResponse<GetParkingLot>(StatusCodes.Status201Created, data: [dto]);
		}

		public async Task<ApiResponse> UpdateAsync(Guid uid, AddParkingLot model, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse> DeleteAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingLot? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.ParkingLotId.Equals(uid));
			if (data is null) return new ApiResponse(StatusCodes.Status400BadRequest);

			if (data.ParkingSpots.Any(spot => spot.Reservations.Any(reservation => reservation.Status.Equals(nameof(ReservationStatus.Active)) || reservation.Status.Equals(nameof(ReservationStatus.Reserved))))
			    ) return new ApiResponse(StatusCodes.Status400BadRequest,
				message: "The lot cannot be deleted because it contains occupied spots or spots with active reservations. Please clear or cancel all reservations before proceeding.");

			data.DeletedAt = DateTime.UtcNow;
			await dbContext.SaveChangesAsync();
			return new ApiResponse(StatusCodes.Status204NoContent);
		}
	}
}
