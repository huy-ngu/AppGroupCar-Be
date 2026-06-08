using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.ShowtimeRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse;

namespace AppGroupCar.Services.Bus.ShowtimeServices
{
    public interface IBusShowtimeService
    {
        Task<IEnumerable<BusShowtimeResponse>> GetAsync();
        Task<BusShowtimeResponse> GetById(Guid id);
        Task<BusShowtimeDetailResponseDto> GetPublicDetailAsync(Guid id);
        Task<BusShowtimeResponse> CreateAsync(
            Guid companyId,
            Guid userId,
            CreateBusShowtimeRequest dto);

        Task<List<BusShowtimeResponse>> GetByCompanyAsync(
            Guid companyId);

        Task<List<UpcomingShowtimeItemDto>> GetUpcomingForCompanyAsync(
            Guid companyId,
            DateOnly fromDate);

        Task<PagedResult<BusShowtimeResponse>> FilterAsync(
            Guid companyId,
            BusShowtimeParameters param);

        Task<BusShowtimeResponse> UpdateAsync(
            Guid id,
            Guid companyId,
            UpdateBusShowtimeRequest dto);

        Task DeleteAsync(
            Guid id,
            Guid companyId,
            Guid userId);

        Task<BusShowtimeResponse> ChangeStatusAsync(
            Guid id,
            Guid companyId,
            Guid userId,
            ChangeBusShowtimeStatusRequest dto);

        Task<PagedResult<BusShowtimeSearchResponse>> SearchAsync(
            BusShowtimeSearchParameters param);
    }
}
