using ParkeoApp.Application.Helpers.Pagination;
using ParkeoApp.Application.Services.TenantService;
using ParkeoApp.Domain.ApiResponseModels;
using ParkeoApp.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ParkeoApp.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TenantsController(ITenantService service) : ControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams qParams)
        {
            var response = await service.GetAllAsync(qParams);
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet("{id:guid}")]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
        {
            var response = await service.GetByIdAsync(id, jwt);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromHeader(Name = "Authorization")] string jwt, [FromBody] AddTenant model)
        {
            var response = await service.CreateAsync(model, jwt);
            return StatusCode(response.StatusCode, response);
        }

        /*[HttpPut("{id:guid}")]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id, [FromBody] AddTenant model)
        {
            var response = await service.UpdateAsync(id, model, jwt);
            return StatusCode(response.StatusCode, response);
        }*/

        /*[HttpDelete("{id:guid}")]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ApiResponse>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync([FromHeader(Name = "Authorization")] string jwt, [FromRoute] Guid id)
        {
            var response = await service.DeleteAsync(id, jwt);
            return StatusCode(response.StatusCode, response);
        }*/


    }
}
