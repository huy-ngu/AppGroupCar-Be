using AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Services.Market.SharedRideOfferDetailServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SharedRideOfferDetailController : ControllerBase
    {
        private readonly ISharedRideOfferDetailService _service;

        public SharedRideOfferDetailController(ISharedRideOfferDetailService service)
        {
            _service = service;
        }

        /// <summary>
        /// L?y danh sách t?t c? SharedRideOfferDetail (phân trang)
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var details = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(new ApiResponse<object>
            {
                Data = new
                {
                    Items = details.Items,
                    Pagination = new
                    {
                        TotalCount = details.TotalCount,
                        PageNumber = details.PageNumber,
                        PageSize = details.PageSize,
                        TotalPages = details.TotalPages,
                        HasPreviousPage = details.HasPreviousPage,
                        HasNextPage = details.HasNextPage
                    }
                }
            });
        }

        /// <summary>
        /// L?y chi ti?t SharedRideOfferDetail theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var detail = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<GetSharedRideOfferDetailDTO> { Data = detail });
        }

        /// <summary>
        /// L?y chi ti?t SharedRideOfferDetail theo OfferId
        /// </summary>
        [HttpGet("by-offer/{offerId}")]
        public async Task<IActionResult> GetByOfferId(Guid offerId)
        {
            var detail = await _service.GetByOfferIdAsync(offerId);
            return Ok(new ApiResponse<GetSharedRideOfferDetailDTO> { Data = detail });
        }

        /// <summary>
        /// T?o SharedRideOfferDetail m?i
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateSharedRideOfferDetailDTO request)
        {
            var detail = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = detail.Id },
                new ApiResponse<GetSharedRideOfferDetailDTO>
                {
                    Data = detail,
                    Message = "SharedRideOfferDetail ?ã ???c t?o thành công"
                });
        }

        /// <summary>
        /// C?p nh?t SharedRideOfferDetail
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSharedRideOfferDetailDTO request)
        {
            var detail = await _service.UpdateAsync(id, request);
            return Ok(new ApiResponse<GetSharedRideOfferDetailDTO>
            {
                Data = detail,
                Message = "SharedRideOfferDetail ?ã ???c c?p nh?t thành công"
            });
        }

        /// <summary>
        /// Xóa SharedRideOfferDetail
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(new ApiResponse<string>
            {
                Message = "SharedRideOfferDetail ?ã ???c xóa thành công"
            });
        }
    }
}
