using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Services.TenantService
{
	public interface ITenantService:IBaseService<GetTenant, AddTenant>
	{
		Task<ApiResponse<GetTenant>> GetAllAsync(PaginationParams paginationParams);

	}
}
