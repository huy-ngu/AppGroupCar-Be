using AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse;
using AppGroupCar.DTO;

namespace AppGroupCar.Services.Bus.CompanyServices
{
    public interface ICompanyService
    {
        Task<CompanyDetailResponseDto?> GetByIdAsync(Guid id);

        Task<PagedResult<CompanyResponseDto>> GetCompaniesAsync(
            int pageNumber,
            int pageSize,
            string? keyword);

        Task<CompanyResponseDto> CreateCompanyAsync(CreateCompanyRequestDto dto);//, Guid userId);

        Task<CompanyResponseDto> UpdateCompanyAsync(Guid id, UpdateCompanyRequestDto dto);

        Task DeleteCompanyAsync(Guid id);

        Task<PagedResult<CompanyResponseDto>> GetPendingCompaniesAsync(int pageNumber, int pageSize);

        Task<CompanyResponseDto> ApproveCompanyAsync(Guid id, ApproveCompanyRequestDto dto);
    }
}
