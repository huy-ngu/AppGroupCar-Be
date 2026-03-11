using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGroupCar.Services.Market.OfferServices
{
    public interface IOfferService
    {
        Task<Offer> CreateAsync(Guid driverId, CreateOfferRequestDTO request);
        Task<Offer> UpdateAsync(Guid driverId, Guid id, UpdateOfferRequestDTO request);
        Task<Offer> GetByIdAsync(Guid id);
        Task<IEnumerable<Offer>> GetAllAsync();
        Task<IEnumerable<Offer>> GetMyOffersAsync(Guid driverId);
        Task DeleteAsync(Guid driverId, Guid id);
    }
}
