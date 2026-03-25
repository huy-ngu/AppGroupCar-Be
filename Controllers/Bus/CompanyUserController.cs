using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Bus.CUResponse;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.UserCompanyServices;
using AppGroupCar.Services.Auth.UserServices;
using AppGroupCar.DTO.RequestDTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AppGroupCar.Controllers.Bus
{
	[ApiController]
	[Route("company-users")]
	[Authorize(Roles = "ADMIN_COMPANY")]
	public class CompanyUserController : ControllerBase
	{
		private readonly IUCService _ucService;
		private readonly IUserService _userService;

		public CompanyUserController(IUCService ucService, IUserService userService)
		{
			_ucService = ucService;
			_userService = userService;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateCompanyStaffDTO request)
		{
			var companyIdNullable = User.GetCompanyId();
			if (companyIdNullable is not Guid companyId)
			{
				return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
			}

			var creatorFullName = User.GetFullName();
			if (string.IsNullOrWhiteSpace(creatorFullName))
			{
				return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
			}

			var userRequest = new CreateUserDTO
			{
				UserName = $"staff{Random.Shared.Next(0, 100):D2}{creatorFullName}",
				Email = request.Email,
				FullName = creatorFullName,
				PhoneNumber = request.PhoneNumber,
				Password = request.PhoneNumber,
				Roles = new List<string> { "STAFF_COMPANY" }
			};

			var user = await _userService.CreateAsync(userRequest);
			var result = await _ucService.CreateAsync(new CreateCURequestDto
			{
				CompanyId = companyId,
				UserId = user.Id
			});

			return Ok(new ApiResponse<CUResponseDto> { Data = result });
		}

		[HttpPost("leave")]
		public async Task<IActionResult> LeaveCompany([FromBody] LeaveCompanyRequestDto? request)
		{
			request ??= new LeaveCompanyRequestDto();

			var companyIdNullable = User.GetCompanyId();
			if (companyIdNullable is not Guid companyId)
			{
				return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
			}

			if (request.UserId != null && request.CompanyId == null)
			{
				request.CompanyId = companyId;
			}

			if (request.CompanyId != null && request.CompanyId != companyId)
			{
				return StatusCode(403, new ApiResponse<string> { Message = "Không có quyền trên công ty này" });
			}

			var hasCompanyUserId = request.CompanyUserId != null;
			var hasUserCompanyPair = request.UserId != null && request.CompanyId != null;

			if (!hasCompanyUserId && !hasUserCompanyPair)
			{
				return BadRequest(new ApiResponse<string>
				{
					Message = "Cần truyền CompanyUserId hoặc cả UserId và CompanyId"
				});
			}

			if ((request.UserId == null) != (request.CompanyId == null))
			{
				return BadRequest(new ApiResponse<string>
				{
					Message = "UserId và CompanyId phải được truyền cùng nhau"
				});
			}

			var success = await _ucService.LeaveCompanyAsync(request);
			if (!success)
			{
				return NotFound(new ApiResponse<string> { Message = "Company user not found" });
			}

			return Ok(new ApiResponse<string> { Message = "Leave company success" });
		}

		[HttpGet("company/users")]
		public async Task<IActionResult> GetUsersByCompany()
		{
			var companyIdNullable = User.GetCompanyId();
			if (companyIdNullable is not Guid tokenCompanyId)
			{
				return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
			}

			var result = await _ucService.GetUsersByCompanyAsync(tokenCompanyId);
			return Ok(new ApiResponse<IEnumerable<CUResponseDto>> { Data = result });
		}

		[HttpPatch("company/{companyId}/users/{userId}/active")]
		public async Task<IActionResult> UpdateActive(
			Guid companyId,
			Guid userId,
			[FromBody] UpdateCUStatusRequestDto request)
		{
			var companyIdNullable = User.GetCompanyId();
			if (companyIdNullable is not Guid tokenCompanyId)
			{
				return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
			}

			if (companyId != tokenCompanyId)
			{
				return StatusCode(403, new ApiResponse<string> { Message = "Không có quyền trên công ty này" });
			}

			var result = await _ucService.GetActiveCompanyByUserAsync(userId, companyId, request);
			return Ok(new ApiResponse<CUResponseDto> { Data = result });
		}
	}
}
