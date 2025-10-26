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
		private IQueryable<tenant> LoadData(Guid tenant) => dbContext.tenants.Where(e => e.is_active && e.tenant_id == tenant)
		.AsQueryable();

		public async Task<ApiResponse<GetTenant>> GetAllAsync(PaginationParams paginationParams, string jwt)
		{
			var tokenPayload = Utils.DecodeJwt(jwt);
			var data =await LoadData(tokenPayload.Tenant).ToListAsync();
			var dto = mapper.Map<ICollection<GetTenant>>(data);

			var pagedItem = PagedList<GetTenant>.ToPagedList(dto,paginationParams.CurrentPage,paginationParams.PageSize);
			return new ApiResponse<GetTenant>(data: pagedItem);
		}

		public async Task<ApiResponse<GetTenant>> GetByIdAsync(string uid, string jwt)
		{
			var tokenPayload = Utils.DecodeJwt(jwt);
			var data =  await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e =>e.tenant_id.Equals(uid));
			if (data is null) return new ApiResponse<GetTenant>(StatusCodes.Status400BadRequest);
			var dto = mapper.Map<GetTenant>(data);
			return new ApiResponse<GetTenant>(data: [dto]);
		}

		public async Task<ApiResponse<GetTenant>> CreateAsync(AddTenant model, string jwt)
		{
			var tokenPayload = Utils.DecodeJwt(jwt);
			tenant newTenant = mapper.Map<tenant>(model);

			newTenant.is_active = true;
			var entry = await dbContext.tenants.AddAsync(newTenant);
			await dbContext.SaveChangesAsync();

			var dto = mapper.Map<GetTenant>(entry.Entity);
			return new ApiResponse<GetTenant>(StatusCodes.Status201Created,data: [dto]);
		}
		public async Task<ApiResponse<GetTenant>> UpdateAsync(string uid, AddTenant model, string jwt) => throw new NotImplementedException();
		public async Task<ApiResponse<GetTenant>> DeleteAsync(string uid, string jwt) => throw new NotImplementedException();
	}
}
