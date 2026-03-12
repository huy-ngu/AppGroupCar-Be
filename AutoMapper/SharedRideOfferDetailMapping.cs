using AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AutoMapper;

namespace AppGroupCar.AutoMapper
{
    public class SharedRideOfferDetailMapping : Profile
    {
        public SharedRideOfferDetailMapping()
        {
            //CreateMap<CreateSharedRideOfferDetailDTO, SharedRideOfferDetail>();

            CreateMap<UpdateSharedRideOfferDetailDTO, SharedRideOfferDetail>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SharedRideOfferDetail, GetSharedRideOfferDetailDTO>();
        }
    }
}
