using AutoMapper;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Application.Helpers.Pagination;

namespace ParkeoApp.Application.Services.UserService
{
	public class UserService(ParkeoAppContext dbContext, IMapper mapper) : IUserService
	{
		private IQueryable<User> LoadData(Guid tenantId) => dbContext.Users
			.Where(e => !e.DeletedAt.HasValue && e.TenantId.Equals(tenantId))
			.Include(e => e.UserRoles.OrderByDescending(e=>e.AssignedAt))
			.Include(e => e.UsersTokens.OrderByDescending(e=>e.CreatedAt))
			.Include(e => e.Reservations.OrderByDescending(e=>e.CreatedAt))
				.ThenInclude(e=> e.Spot).ThenInclude(e=> e.ParkingLot)
			.OrderByDescending(e => e.UpdatedAt).ThenByDescending(e => e.CreatedAt)
			.AsQueryable();

		public async Task<ApiResponse<GetUser>> GetAllAsync(PaginationParams paginationParams, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			var data = await LoadData(tokenPayload.Tenant).ToListAsync();
			var dto = mapper.Map<ICollection<GetUser>>(data);

			var pagedItem = PagedList<GetUser>.ToPagedList(dto, paginationParams.CurrentPage, paginationParams.PageSize);
			return new ApiResponse<GetUser>(data: pagedItem);
		}

		public async Task<ApiResponse<GetUser>> GetByIdAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			User? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.UserId.Equals(uid));
			if (data is null) return new ApiResponse<GetUser>(StatusCodes.Status400BadRequest);
			GetUser? dto = mapper.Map<GetUser>(data);
			return new ApiResponse<GetUser>(data: [dto]);
		}

		public async Task<ApiResponse<GetUser>> CreateAsync(AddUser model, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			User newUser = mapper.Map<User>(model);

			var entry = await dbContext.Users.AddAsync(newUser);
			await dbContext.SaveChangesAsync();

			GetUser? dto = mapper.Map<GetUser>(entry.Entity);
			return new ApiResponse<GetUser>(StatusCodes.Status201Created, data: [dto]);
		}

		public async Task<ApiResponse> UpdateAsync(Guid uid, AddUser model, string jwt) => throw new NotImplementedException();

		public async Task<ApiResponse> DeleteAsync(Guid uid, string jwt)
		{
			TokenPayload tokenPayload = Utils.DecodeJwt(jwt);
			User? data = await LoadData(tokenPayload.Tenant).FirstOrDefaultAsync(e => e.UserId.Equals(uid));
			if (data is null) return new ApiResponse(StatusCodes.Status400BadRequest);
			data.DeletedAt = DateTime.UtcNow;
			return new ApiResponse(StatusCodes.Status204NoContent);
		}
	}
}
