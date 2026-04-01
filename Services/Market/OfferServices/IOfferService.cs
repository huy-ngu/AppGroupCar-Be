using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGroupCar.Services.Market.OfferServices
{
    public interface IOfferService
    {
        Task<GetOfferDTO> CreateWithDetailAsync(Guid driverId, CreateOfferSharedRidelRequestDTO request);
        Task<GetOfferDTO> UpdateAsync(Guid driverId, Guid id, UpdateOfferRequestDTO request);
        Task<GetOfferDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<GetOfferDTO>> GetAllAsync();
        Task<IEnumerable<GetOfferDTO>> GetMyOffersAsync(Guid driverId);
        Task DeleteAsync(Guid driverId, Guid id);
    }
}
