using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Services.UserService
{
	public interface IUserService:IBaseService<GetUser, AddUser>
	{
		Task<ApiResponse<GetReservationWNRef>> GetMyReservationsAsync(PaginationParams paginationParams, string jwt);
	}
}
