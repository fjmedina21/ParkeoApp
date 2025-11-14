using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.RoleService
{
	public class RoleService(ParkeoAppContext dbContext, IMapper mapper) : IRoleService
	{
		private IQueryable<Role> LoadData(Guid tenantId) => dbContext.Roles
			.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(tenantId))
			.Include(e => e.UserRoles.OrderByDescending(e=>e.AssignedAt))
			.OrderByDescending(e => e.UpdatedAt).ThenByDescending(e => e.CreatedAt)
			.AsQueryable();

		public async Task<ApiResponse<GetRole>> GetAllAsync(PaginationParams paginationParams, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			var data =await LoadData(tokenPayload.Tenant).ToListAsync();
			var dto = mapper.Map<ICollection<GetRole>>(data);

			var pagedItem = PagedList<GetRole>.ToPagedList(dto,paginationParams.CurrentPage,paginationParams.PageSize);
			return new ApiResponse<GetRole>(data: pagedItem);
		}

		public async Task<ApiResponse<GetRole>> GetByIdAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Role? data =  await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e =>e.RoleId.Equals(uid));
			if (data is null) return new ApiResponse<GetRole>(StatusCodes.Status400BadRequest);
			GetRole? dto = mapper.Map<GetRole>(data);
			return new ApiResponse<GetRole>(data: [dto]);
		}

		public async Task<ApiResponse<GetRole>> CreateAsync(AddRole model, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Role newRole = mapper.Map<Role>(model);

			var entry = await dbContext.Roles.AddAsync(newRole);
			await dbContext.SaveChangesAsync();

			GetRole? dto = mapper.Map<GetRole>(entry.Entity);
			return new ApiResponse<GetRole>(StatusCodes.Status201Created,data: [dto]);
		}

		public async Task<ApiResponse> UpdateAsync(Guid uid, AddRole model, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse> DeleteAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			Role? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.RoleId.Equals(uid));
			if (data is null) return new ApiResponse(StatusCodes.Status400BadRequest);
			data.DeletedAt = DateTime.UtcNow;
			await dbContext.SaveChangesAsync();
			return new ApiResponse(message: "deleted successfully.");
		}
	}
}
