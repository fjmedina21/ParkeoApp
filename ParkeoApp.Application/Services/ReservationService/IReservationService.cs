using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Services.ReservationService
{
	public interface IReservationService
	{
		Task<ApiResponse<GetReservation>> ReserveAsync(AddReservation reservation, string jwt);
		Task<ApiResponse<GetReservation>> CheckInAsync(string reservationCode, string jwt);
		Task<ApiResponse<GetReservation>> CheckOutAsync(string reservationCode, string jwt);
		Task<ApiResponse<GetReservation>> CancelAsync(string reservationCode, string jwt);
		Task<ApiResponse<GetReservation>> GetByCodeAsync(string reservationCode, string jwt);
		Task<ApiResponse<GetReservation>> GetByUserAsync(PaginationParams paginationParams,string jwt);

	}
}
