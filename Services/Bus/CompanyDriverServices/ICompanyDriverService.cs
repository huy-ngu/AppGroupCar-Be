using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyDriverResponse;

namespace AppGroupCar.Services.Bus.CompanyDriverServices
{
    public interface ICompanyDriverService
    {
        Task<CompanyDriverDetailResponseDto?> GetByIdAsync(Guid id, Guid companyId);
        Task<PagedResult<CompanyDriverResponseDto>> GetByCompanyAsync(CompanyDriverParameters param, Guid companyId);
        Task<CompanyDriverResponseDto> CreateAsync(Guid companyId, CreateCompanyDriverRequestDto dto);
        Task<CompanyDriverResponseDto> UpdateAsync(Guid id, Guid companyId, UpdateCompanyDriverRequestDto dto);
        Task DeleteAsync(Guid id, Guid companyId);
    }
}
