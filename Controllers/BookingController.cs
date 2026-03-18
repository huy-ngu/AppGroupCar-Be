using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.Booking;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Services.Market.BookingServices;
using Azure;
using Microsoft.AspNetCore.Mvc;
namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _bookingService.GetAllAsync();
            return Ok(new ApiResponse<object> { Data = response });

        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _bookingService.GetByIdAsync(id);
            return Ok(new ApiResponse<object> { Data = response });
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingRequestDTO request)
        {
            var response = await _bookingService.CreateAsync(request);

            return Ok(new ApiResponse<object> { Code = 201, Data = response, Message = "Create success" });
        }
        [HttpPost("create_with_items")]
        public async Task<IActionResult> CreateWithItems(CreateWithBookingIteamsRequestDTO request)
        {
            await _bookingService.CreateWithBookingIteamsAsync(request);
            return Ok(new ApiResponse<object> { Code = 201, Message = "Create success" });

        }
        [HttpPatch]
        public async Task<IActionResult> Update(UpdateBookingRequestDTO request)
        {
            await _bookingService.UpdateAsync(request);
            return Ok(new ApiResponse<object> { });

        }
    }
}
