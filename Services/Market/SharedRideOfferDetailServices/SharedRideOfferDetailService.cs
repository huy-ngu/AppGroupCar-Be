using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;

namespace AppGroupCar.Services.Market.SharedRideOfferDetailServices
{
    public class SharedRideOfferDetailService : ISharedRideOfferDetailService
    {
        private readonly IBaseRepository<SharedRideOfferDetail> _repo;
        private readonly IBaseRepository<Offer> _offerRepo;
        private readonly IMapper _mapper;

        public SharedRideOfferDetailService(
            IBaseRepository<SharedRideOfferDetail> repo,
            IBaseRepository<Offer> offerRepo,
            IMapper mapper)
        {
            _repo = repo;
            _offerRepo = offerRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<GetSharedRideOfferDetailDTO>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _repo.FindAndFilter(
                predicate: s => s.DeleteAt == null,
                orderBy: q => q.OrderByDescending(s => s.CreatedAt),
                pageNumber,
                pageSize
            );

            var dtos = result.Items.Select(s => new GetSharedRideOfferDetailDTO
            {
                Id = s.Id,
                TotalSeats = s.TotalSeats,
                AvailableSeats = s.AvailableSeats
            }).ToList();

            return new PagedResult<GetSharedRideOfferDetailDTO>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<GetSharedRideOfferDetailDTO> GetByIdAsync(Guid id)
        {
            var detail = await _repo.GetByIdAsync(id);

            if (detail == null || detail.DeleteAt != null)
            {
                throw new AppException(404, "SharedRideOfferDetail không tìm thấy");
            }

            return new GetSharedRideOfferDetailDTO
            {
                Id = detail.Id,
                TotalSeats = detail.TotalSeats,
                AvailableSeats = detail.AvailableSeats
            };
        }

        public async Task<GetSharedRideOfferDetailDTO> GetByOfferIdAsync(Guid offerId)
        {
            var details = await _repo.FindAsync(
                s => s.DeleteAt == null,
                1,
                int.MaxValue
            );

            var detail = details.FirstOrDefault();
            if (detail == null)
            {
                throw new AppException(404, "SharedRideOfferDetail không tìm thấy cho Offer này");
            }

            return new GetSharedRideOfferDetailDTO
            {
                Id = detail.Id,
                TotalSeats = detail.TotalSeats,
                AvailableSeats = detail.AvailableSeats
            };
        }

        public async Task<GetSharedRideOfferDetailDTO> CreateAsync(CreateSharedRideOfferDetailDTO request)
        {
            // Kiểm tra Offer có tồn tại không
            var offer = await _offerRepo.GetByIdAsync(request.OfferId);
            if (offer == null || offer.DeleteAt != null)
            {
                throw new AppException(404, "Offer không tồn tại");
            }

            // Kiểm tra xem đã có SharedRideOfferDetail cho Offer này chưa
            var existingDetail = await _repo.FindAsync(
                s => s.DeleteAt == null && s.Id == offer.Id,
                1,
                int.MaxValue
            );

            if (existingDetail.Any())
            {
                throw new AppException(400, "Offer này đã có SharedRideOfferDetail");
            }

            // Validate AvailableSeats <= TotalSeats
            if (request.AvailableSeats > request.TotalSeats)
            {
                throw new AppException(400, "AvailableSeats không thể lớn hơn TotalSeats");
            }

            var detail = new SharedRideOfferDetail
            {
                Id = request.OfferId,
                TotalSeats = request.TotalSeats,
                AvailableSeats = request.AvailableSeats
            };

            await _repo.AddAsync(detail);
            await _repo.SaveChangesAsync();

            return new GetSharedRideOfferDetailDTO
            {
                Id = detail.Id,
                TotalSeats = detail.TotalSeats,
                AvailableSeats = detail.AvailableSeats
            };
        }

        public async Task<GetSharedRideOfferDetailDTO> UpdateAsync(Guid id, UpdateSharedRideOfferDetailDTO request)
        {
            var detail = await _repo.GetByIdAsync(id);

            if (detail == null || detail.DeleteAt != null)
            {
                throw new AppException(404, "SharedRideOfferDetail không tìm thấy");
            }

            // Cập nhật TotalSeats nếu có
            if (request.TotalSeats.HasValue)
            {
                detail.TotalSeats = request.TotalSeats.Value;
            }

            // Cập nhật AvailableSeats nếu có
            if (request.AvailableSeats.HasValue)
            {
                detail.AvailableSeats = request.AvailableSeats.Value;
            }

            // Validate AvailableSeats <= TotalSeats
            if (detail.AvailableSeats > detail.TotalSeats)
            {
                throw new AppException(400, "AvailableSeats không thể lớn hơn TotalSeats");
            }

            _repo.Update(detail);
            await _repo.SaveChangesAsync();

            return new GetSharedRideOfferDetailDTO
            {
                Id = detail.Id,
                TotalSeats = detail.TotalSeats,
                AvailableSeats = detail.AvailableSeats
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var detail = await _repo.GetByIdAsync(id);

            if (detail == null || detail.DeleteAt != null)
            {
                throw new AppException(404, "SharedRideOfferDetail không tìm thấy");
            }

            _repo.Delete(detail);
            await _repo.SaveChangesAsync();
        }

    }
}
