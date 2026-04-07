using AppGroupCar.DTO.ResponseDTO.Bus.CompanyVehicleResponse;
using AppGroupCar.DTO.RequestDTO.Bus.VehicleRequest;

namespace AppGroupCar.Services.Bus.CompanyVehicleServices
{
    public interface ICompanyVehicleService
    {
        Task<CompanyVehicleDetailResponseDto?> GetByIdAsync(Guid id, Guid companyId);
        Task<List<CompanyVehicleResponseDto>> GetByCompanyAsync(Guid companyId);
        Task<CompanyVehicleDetailResponseDto> CreateAsync(Guid companyId, CreateCompanyVehicleRequest dto);
        Task<CompanyVehicleResponseDto> UpdateAsync(Guid id, Guid companyId, UpdateCompanyVehicleRequest dto);
        Task DeleteAsync(Guid id, Guid companyId);
        Task<string> GetSeatLayoutJsonByVehicleAsync(Guid companyVehicleId, Guid companyId);
    }
}
