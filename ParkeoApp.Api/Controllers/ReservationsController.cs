using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkeoApp.Application.Services.ParkingSpotService;
using ParkeoApp.Application.Services.ReservationService;

namespace ParkeoApp.Api.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class ReservationsController(IReservationService service) : ControllerBase
	{
		[HttpGet]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetByUserAsync([FromHeader(Name = "Authorization")] string jwt, [FromQuery] PaginationParams qParams)
		{
			var response = await service.GetByUserAsync(qParams, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{code}")]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByIdAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] string code)
		{
			var response = await service.GetByCodeAsync(code, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status201Created)]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ReserveAsync([FromHeader(Name = "Authorization")] string jwt, [FromBody] AddReservation model)
		{
			var response = await service.ReserveAsync(model, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost("{code}/check-in")]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CheckInAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] string code)
		{
			var response = await service.CheckInAsync(code, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost("{code}/check-out")]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CheckOutAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] string code)
		{
			var response = await service.CheckOutAsync(code, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost("{code}/cancel")]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetReservation>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CancelAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] string code)
		{
			var response = await service.CancelAsync(code, jwt);
			return StatusCode(response.StatusCode, response);
		}


	}
}
