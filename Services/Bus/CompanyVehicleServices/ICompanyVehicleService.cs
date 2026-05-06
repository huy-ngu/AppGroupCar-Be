using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.VehicleRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyVehicleResponse;

namespace AppGroupCar.Services.Bus.CompanyVehicleServices
{
    public interface ICompanyVehicleService
    {
        Task<CompanyVehicleDetailResponseDto?> GetByIdAsync(Guid id, Guid companyId);
        Task<PagedResult<CompanyVehicleResponseDto>> GetByCompanyAsync(CompanyVehicleParameters param, Guid companyId);
        Task<CompanyVehicleDetailResponseDto> CreateAsync(Guid companyId, CreateCompanyVehicleRequest dto);
        Task<CompanyVehicleResponseDto> UpdateAsync(Guid id, Guid companyId, UpdateCompanyVehicleRequest dto);
        Task DeleteAsync(Guid id, Guid companyId);
        Task<string> GetSeatLayoutJsonByVehicleAsync(Guid companyVehicleId, Guid companyId);
    }
}
