using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ParkingSpotService
{
	public class ParkingSpotService(ParkeoAppContext dbContext, IMapper mapper) : IParkingSpotService
	{
		private IQueryable<ParkingSpot> LoadData(Guid tenantId) => dbContext.ParkingSpots
			.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(tenantId))
			.Include(e => e.ParkingLot)
			.Include(e => e.Reservations.OrderByDescending(e=>e.CreatedAt))
			.OrderByDescending(e => e.UpdatedAt).ThenByDescending(e => e.CreatedAt)
			.AsQueryable();

		public async Task<ApiResponse<GetParkingSpot>> GetAllAsync(PaginationParams paginationParams, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			var data =await LoadData(tokenPayload.Tenant).ToListAsync();
			var dto = mapper.Map<ICollection<GetParkingSpot>>(data);

			var pagedItem = PagedList<GetParkingSpot>.ToPagedList(dto,paginationParams.CurrentPage,paginationParams.PageSize);
			return new ApiResponse<GetParkingSpot>(data: pagedItem);
		}

		public async Task<ApiResponse<GetParkingSpot>> GetByIdAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingSpot? data =  await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e =>e.SpotId.Equals(uid));
			if (data is null) return new ApiResponse<GetParkingSpot>(StatusCodes.Status400BadRequest);
			GetParkingSpot? dto = mapper.Map<GetParkingSpot>(data);
			return new ApiResponse<GetParkingSpot>(data: [dto]);
		}

		public async Task<ApiResponse<GetParkingSpot>> CreateAsync(AddParkingSpot model, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingSpot newParkingSpot = mapper.Map<ParkingSpot>(model);

			newParkingSpot.TenantId = tokenPayload.Tenant;
			var entry = await dbContext.ParkingSpots.AddAsync(newParkingSpot);
			await dbContext.SaveChangesAsync();

			GetParkingSpot? dto = mapper.Map<GetParkingSpot>(entry.Entity);
			return new ApiResponse<GetParkingSpot>(StatusCodes.Status201Created,data: [dto]);
		}

		public async Task<ApiResponse> UpdateAsync(Guid uid, AddParkingSpot model, string jwt) => throw new NotImplementedException();

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
