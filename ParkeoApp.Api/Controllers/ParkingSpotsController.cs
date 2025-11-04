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
		[HttpGet]
		[ProducesResponseType<ApiResponse<GetParkingSpot>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetParkingSpot>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetAllAsync([FromHeader(Name = "Authorization")] string jwt, [FromQuery] PaginationParams qParams)
		{
			HttpContext ht = HttpContext;
			var response = await service.GetAllAsync(qParams, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id:guid}")]
		[ProducesResponseType<ApiResponse<GetParkingSpot>>(StatusCodes.Status200OK)]
		[ProducesResponseType<ApiResponse<GetParkingSpot>>(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByIdAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
		{
			var response = await service.GetByIdAsync(id, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		[ProducesResponseType<ApiResponse<GetParkingSpot>>(StatusCodes.Status201Created)]
		[ProducesResponseType<ApiResponse<GetParkingSpot>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAsync([FromHeader(Name = "Authorization")] string jwt, [FromBody] AddParkingSpot model)
		{
			var response = await service.CreateAsync(model, jwt);
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
		[ProducesResponseType<ApiResponse>(StatusCodes.Status204NoContent)]
		[ProducesResponseType<ApiResponse>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
		{
			var response = await service.DeleteAsync(id, jwt);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost("{id:guid}/create-reservation")]
		[ProducesResponseType<ApiResponse<GetReservationWNRef>>(StatusCodes.Status201Created)]
		[ProducesResponseType<ApiResponse<GetReservationWNRef>>(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateReservationAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id, [FromBody] AddReservation model)
		{
			var response = await service.CreateReservationAsync(id, model, jwt);
			return StatusCode(response.StatusCode, response);
		}
	}
}
