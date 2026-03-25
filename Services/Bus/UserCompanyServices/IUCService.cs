using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CUResponse;

namespace AppGroupCar.Services.Bus.UserCompanyServices
{
    public interface IUCService
    {
        // hàm để tạo đối tượng CompanyUser mới, trả về CUResponseDto chứa thông tin của đối tượng đã tạo
        Task<CUResponseDto>
            CreateAsync(
                CreateCURequestDto request);

        // hàm để người dùng rời khỏi công ty là xóa mềm bằng cách chuyển DeleteAt khác null
        Task<bool>
            LeaveCompanyAsync(
                LeaveCompanyRequestDto request);

        // hàm để lấy tất cả người dùng thuộc một công ty, trả về một danh sách CUResponseDto chứa thông tin của các người dùng
        Task<IEnumerable<CUResponseDto>>
            GetUsersByCompanyAsync(
                Guid companyId);

        // hàm để cập nhật thông tin staff theo UserId và CompanyId
        Task<CUResponseDto>
            GetActiveCompanyByUserAsync(
                Guid userId,
                Guid companyId,
                UpdateCUStatusRequestDto request);
    }
}
