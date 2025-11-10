using AutoMapper;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;

namespace ParkeoApp.Application.Services.TenantService
{
	public class TenantService(ParkeoAppContext dbContext, IMapper mapper) : ITenantService
	{
		private IQueryable<Tenant> LoadData() => dbContext.Tenants
			.Where(e => !e.DeletedAt.HasValue)
			.OrderByDescending(e => e.UpdatedAt).ThenByDescending(e => e.CreatedAt)
			.AsQueryable();

		public async Task<ApiResponse<GetTenant>> GetAllAsync(PaginationParams paginationParams)
		{
			var data =await LoadData().ToListAsync();
			var dto = mapper.Map<ICollection<GetTenant>>(data);

			var pagedItem = PagedList<GetTenant>.ToPagedList(dto,paginationParams.CurrentPage,paginationParams.PageSize);
			return new ApiResponse<GetTenant>(data: pagedItem);
		}

		public Task<ApiResponse<GetTenant>> GetAllAsync(PaginationParams paginationParams, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse<GetTenant>> GetByIdAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Tenant? data =  await LoadData().FirstOrDefaultAsync(e =>e.TenantId.Equals(uid));
			if (data is null) return new ApiResponse<GetTenant>(StatusCodes.Status400BadRequest);
			GetTenant? dto = mapper.Map<GetTenant>(data);
			return new ApiResponse<GetTenant>(data: [dto]);
		}

		public async Task<ApiResponse<GetTenant>> CreateAsync(AddTenant model, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Tenant newTenant = mapper.Map<Tenant>(model);

			var entry = await dbContext.Tenants.AddAsync(newTenant);
			await dbContext.SaveChangesAsync();

			GetTenant? dto = mapper.Map<GetTenant>(entry.Entity);
			return new ApiResponse<GetTenant>(StatusCodes.Status201Created,data: [dto]);
		}
		public async Task<ApiResponse> UpdateAsync(Guid uid, AddTenant model, string jwt) => throw new NotImplementedException();
		public async Task<ApiResponse> DeleteAsync(Guid uid, string jwt) => throw new NotImplementedException();
	}
}
