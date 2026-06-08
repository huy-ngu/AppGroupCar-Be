using AppGroupCar.DTO;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Services.Market.BookingOfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class BookingOfferController : ControllerBase
    {
        private readonly IBookingOfferService _bookingOfferService;

        public BookingOfferController(IBookingOfferService bookingOfferService)
        {
            _bookingOfferService = bookingOfferService;
        }

        [HttpPost]
        [Authorize(Roles = "Driver,User")]
        public async Task<IActionResult> CreateBookingOffer([FromQuery] Guid rideRequestId, [FromQuery] Guid offerId)
        {
            var data = await _bookingOfferService.CreateAsync(rideRequestId, offerId);
            return Ok(new ApiResponse<BookingOfferResponseDTO> { Data = data });
        }

        [HttpGet("ride-request/{rideRequestId}")]
        [Authorize]
        public async Task<IActionResult> GetByRideRequest(Guid rideRequestId)
        {
            var data = await _bookingOfferService.GetByRideRequestAsync(rideRequestId);
            return Ok(new ApiResponse<IEnumerable<BookingOfferResponseDTO>> { Data = data });
        }

        [HttpPost("{id}/accept")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AcceptBookingOffer(Guid id)
        {
            await _bookingOfferService.AcceptAsync(id);
            return Ok(new ApiResponse<string> { Message = "Booking offer accepted successfully." });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RejectBookingOffer(Guid id)
        {
            await _bookingOfferService.RejectAsync(id);
            return Ok(new ApiResponse<string> { Message = "Booking offer rejected successfully." });
        }
    }
}
