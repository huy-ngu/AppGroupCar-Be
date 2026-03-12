using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.Entities.Market;
using AutoMapper;

namespace AppGroupCar.AutoMapper
{
    public class ShipmentRequestMapping : Profile
    {
        public ShipmentRequestMapping()
        {
            // 1. Cấu hình cho việc Cập nhật (Update)
            CreateMap<UpdateShipmentRequestDTO, ShipmentRequest>()
                // Bỏ qua ID chính của thực thể (thường không cho phép update ID qua DTO)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                // Bỏ qua ImageUrl theo yêu cầu
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                // Bỏ qua các khóa ngoại (ví dụ: CustomerId, DriverId...)
                // Bạn cần liệt kê cụ thể tên các thuộc tính khóa ngoại ở đây
                .ForMember(dest => dest.PickupLocationId, opt => opt.Ignore())
                .ForMember(dest => dest.DropoffLocationId, opt => opt.Ignore())
                // Chỉ map các giá trị không null từ DTO sang Entity
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // 2. Cấu hình cho việc Tạo mới (Create)
            CreateMap<CreateShipmentRequestDTO, ShipmentRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                // Giả sử bạn muốn bỏ qua các FK khi tạo mới để xử lý logic riêng ở Service
                .ForMember(dest => dest.PickupLocationId, opt => opt.Ignore())
                .ForMember(dest => dest.DropoffLocationId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
