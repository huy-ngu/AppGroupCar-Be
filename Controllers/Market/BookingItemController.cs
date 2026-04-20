using AppGroupCar.DTO.RequestDTO.Market.BookingItem;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AppGroupCar.Services.Market.BookingItemServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class BookingItemController : ControllerBase
    {
        private readonly IBookingItemService _service;

        public BookingItemController(IBookingItemService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateBookingItemRequestDTO request)
        {
            var bookingItem = await _service.CreateBookingItemAsync(request);
            return Ok(new ApiResponse<BookingItem>
            {
                Data = bookingItem
            });

        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var bookingItem = await _service.GetBookingItemByIdAsync(id);
            return Ok(new ApiResponse<BookingItem>
            {
                Data = bookingItem
            });

        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            var bookingItems = await _service.GetAllBookingItemsAsync();
            return Ok(new ApiResponse<IEnumerable<BookingItem>>
            {
                Data = bookingItems
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateBookingItemRequestDTO request)
        {

            await _service.UpdateBookingItemAsync(request);
            return Ok(new ApiResponse<BookingItem>
            {

            });
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteBookingItemAsync(id);
            return Ok(new ApiResponse<bool>
            {
                Data = result
            });
        }
    }
}
