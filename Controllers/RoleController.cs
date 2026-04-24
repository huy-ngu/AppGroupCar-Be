using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Services.Auth.RoleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize("Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRole();
            return Ok(new ApiResponse<List<RoleResponseDTO>> { Data = roles });
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            await _roleService.CreateRole(roleName);
            return Ok(new ApiResponse<string> { });
        }
        [HttpDelete("Id")]
        public async Task<IActionResult> DeleteRole(Guid Id)
        {
            await _roleService.DeleteRole(Id);
            return Ok(new ApiResponse<string> { });
        }
    }
}
