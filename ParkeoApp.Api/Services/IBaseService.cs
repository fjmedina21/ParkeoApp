using ParkeoApp.Api.Helpers.Pagination;
using ParkeoApp.Api.Models.ApiResponses;

namespace ParkeoApp.Api.Services
{
	public interface IBaseService <T, T1> where T : class
	{
		Task<ApiResponse< T>> GetAllAsync(PaginationParams paginationParams, string jwt);
		Task<ApiResponse<T>> GetByIdAsync(string uid,string jwt);
		Task<ApiResponse<T>> CreateAsync(T1 model,string jwt);
		Task<ApiResponse<T>> UpdateAsync(string uid, T1 model,string jwt);
		Task<ApiResponse<T>> DeleteAsync(string uid,string jwt);
	}
}
