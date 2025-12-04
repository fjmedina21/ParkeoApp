using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkeoApp.Application.Services.ParkingSpotService;

namespace ParkeoApp.Api.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class ParkingSpotsController(IParkingSpotService service) : ControllerBase
	{
		[HttpGet("lot/{parkingLotId:guid}")]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetByParkingLotAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid parkingLotId, [FromQuery] PaginationParams qParams)
		{
			var response = await service.GetByParkingLotAsync(parkingLotId, qParams, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id:guid}")]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByIdAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
		{
			var response = await service.GetByIdAsync(id, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status201Created)]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAsync([FromHeader(Name = "Authorization")] string jwt, [FromBody] AddParkingSpot model)
		{
			var response = await service.CreateAsync(model, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost("{id:guid}/mark-as-maintenance")]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> MarkSpotAsMaintenance([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
		{
			var response = await service.MarkSpotAsMaintenanceAsync(id, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost("{id:guid}/mark-as-available")]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetParkingSpotWnRef>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> MarkSpotAsAvailable([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
		{
			var response = await service.MarkSpotAsAvailableAsync(id, jwt);
			return StatusCode(response.StatusCode, response);
		}

		/*
		[HttpPut("{id:guid}")]
		[ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id, [FromBody] AddParkingSpot model)
		{
			var response = await service.UpdateAsync(id, model, jwt);
			return StatusCode(response.StatusCode, response);
		}
		*/

		[HttpDelete("{id:guid}")]
		[ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
		{
			var response = await service.DeleteAsync(id, jwt);
			return StatusCode(response.StatusCode, response);
		}
	}
}
