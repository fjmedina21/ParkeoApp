using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;

namespace ParkeoApp.Application.Services
{
	public interface IBaseService <T, T1> where T : class
	{
		Task<ApiResponse<T>> GetAllAsync(PaginationParams paginationParams, string jwt);
		Task<ApiResponse<T>> GetByIdAsync(Guid uid, string jwt);
		Task<ApiResponse<T>> CreateAsync(T1 model, string jwt);
		Task<ApiResponse> UpdateAsync(Guid uid, T1 model, string jwt);
		Task<ApiResponse> DeleteAsync(Guid uid, string jwt);
	}
}
