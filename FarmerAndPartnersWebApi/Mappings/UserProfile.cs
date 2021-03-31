using AutoMapper;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.ViewModels;

namespace FarmerAndPartnersWebApi.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>()
                .ForMember(dest => dest.Company, opt => opt.Ignore());
        }
    }
}
