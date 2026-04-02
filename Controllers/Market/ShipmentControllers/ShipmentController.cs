using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Services.Market.ShipmentServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market.ShipmentControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController(IShipmentService shipmentService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateFromShipmentOffer(CreateShipmentFromShipmentOfferDTO request)
        {
            var data = await shipmentService.CreateFromShipmentOffer(request);
            return Ok(new ApiResponse<GetShipmentDTO> { Code = 201, Data = data });

        }
    }
}
