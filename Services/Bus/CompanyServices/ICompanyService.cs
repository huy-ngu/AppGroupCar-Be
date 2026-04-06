using AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse;
using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.Entities.Market;
using AppGroupCar.DTO.Filters.Bus;

namespace AppGroupCar.Services.Bus.CompanyServices
{
    public interface ICompanyService
    {
        Task<CompanyDetailResponseDto?> GetByIdAsync(Guid id);

        public Task<PagedResult<CompanyResponseDto>> FilterAsync(CompanyParameters param);

        Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto dto);//, Guid userId);

        Task<CompanyResponseDto> UpdateCompanyAsync(Guid id, UpdateCompanyRequestDto dto);

        Task DeleteCompanyAsync(Guid id);

        public Task<PagedResult<CompanyResponseDto>> GetPendingCompaniesAsync(
    CompanyParameters param);

        Task<CompanyResponseDto> ApproveCompanyAsync(Guid id, ApproveCompanyRequestDto dto);
    }
}
