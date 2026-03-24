using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CUResponse;

namespace AppGroupCar.Services.Bus.UserCompanyServices
{
    public interface IUCService
    {
        public interface ICompanyUserService
        {
            Task<CUResponseDto>
                CreateAsync(
                    CreateCURequestDto request);

            Task<bool>
                LeaveCompanyAsync(
                    Guid userId);

            Task<IEnumerable<CUResponseDto>>
                GetUsersByCompanyAsync(
                    Guid companyId);

            Task<CUResponseDto?>
                GetActiveCompanyByUserAsync(
                    Guid userId);
        }
    }
}
