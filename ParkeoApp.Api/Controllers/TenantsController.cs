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
    public class TenantsController(ITenantService tenantService) : ControllerBase
    {

        [HttpGet]
        [Authorize]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<GetTenant>>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromHeader(Name = "Authorization")] string jwt, [FromQuery] PaginationParams qParams)
        {
            HttpContext ht = HttpContext;
            var response = await tenantService.GetAllAsync(qParams,jwt);
            return StatusCode(response.StatusCode, response);
        }


    }
}
