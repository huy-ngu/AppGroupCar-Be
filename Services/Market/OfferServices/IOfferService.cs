using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGroupCar.Services.Market.OfferServices
{
    public interface IOfferService
    {
        Task<GetOfferDTO> CreateWithDetailAsync(Guid driverId, CreateOfferSharedRidelRequestDTO request);
        Task<GetOfferDTO> UpdateAsync(Guid driverId, UpdateOfferRequestDTO request);
        Task<GetOfferDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<GetOfferDTO>> GetAllAsync();
        Task<IEnumerable<GetOfferDTO>> GetMyOffersAsync(Guid driverId);
        Task DeleteAsync(Guid driverId, Guid id);
        Task<PagedResult<GetOfferSharedRideDTO>> GetOfferSharedRideAsync(GetOfferForUserRequestDTO request);
        Task<GetOfferDTO> CancelOfferAsync(Guid driverId, Guid offerId);
    }
}
