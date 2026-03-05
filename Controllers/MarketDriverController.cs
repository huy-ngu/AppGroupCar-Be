using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.MarketDriverServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketDriverController : ControllerBase
    {
        private readonly IMarketDriverService _marketDriverService;
        public MarketDriverController(IMarketDriverService marketDriverService)
        {
            _marketDriverService = marketDriverService;
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _marketDriverService.GetByIdAsync(id);
            return Ok(new ApiResponse<Driver> { Data = data });
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create(Guid id, CreateDriverRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var driver = await _marketDriverService.CreatAsync(id, request);
            return Ok(new ApiResponse<Driver> { Data = driver });
        }
        [HttpPatch("update")]
        public async Task<IActionResult> Update(UpdateDriverRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            if (request.Id != userId)
            {
                return BadRequest(new ApiResponse<string> { Code = 403, Message = "Không được sửa bản khi người khác" });
            }
            var driver = await _marketDriverService.UpdateAsync(request);
            return Ok(new ApiResponse<Driver> { Data = driver });

        }
    }
}
