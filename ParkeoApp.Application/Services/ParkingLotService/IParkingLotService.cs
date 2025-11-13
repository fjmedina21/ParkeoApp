using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;

namespace ParkeoApp.Application.Services.ParkingLotService
{
	public interface IParkingLotService:IBaseService<GetParkingLot, AddParkingLot>
	{
		Task<ApiResponse<GetParkingLot>> GetAllAsync(PaginationParams paginationParams, string jwt, double? originLat = null,
			double? originLng = null);
	}
}
