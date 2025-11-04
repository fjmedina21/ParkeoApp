using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ParkingLotService
{
	public class ParkingLotService(ParkeoAppContext dbContext, IMapper mapper) : IParkingLotService
	{
		private IQueryable<ParkingLot> LoadData(Guid tenantId) => dbContext.ParkingLots
			.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(tenantId))
			.Include(e => e.ParkingSpots)
			.AsQueryable();

		public async Task<ApiResponse<GetParkingLot>> GetAllAsync(PaginationParams paginationParams, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			var data =await LoadData(tokenPayload.Tenant).ToListAsync();
			var dto = mapper.Map<ICollection<GetParkingLot>>(data);

			var pagedItem = PagedList<GetParkingLot>.ToPagedList(dto,paginationParams.CurrentPage,paginationParams.PageSize);
			return new ApiResponse<GetParkingLot>(data: pagedItem);
		}

		public async Task<ApiResponse<GetParkingLot>> GetByIdAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingLot? data =  await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e =>e.ParkingLotId.Equals(uid));
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
			return new ApiResponse<GetParkingLot>(StatusCodes.Status201Created,data: [dto]);
		}

		public async Task<ApiResponse> UpdateAsync(Guid uid, AddParkingLot model, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse> DeleteAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			ParkingLot? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.ParkingLotId.Equals(uid));
			if (data is null) return new ApiResponse(StatusCodes.Status400BadRequest);
			data.DeletedAt = DateTime.UtcNow;
			return new ApiResponse(StatusCodes.Status204NoContent);
		}
	}
}
