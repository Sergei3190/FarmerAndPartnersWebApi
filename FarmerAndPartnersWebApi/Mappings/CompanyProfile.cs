using AutoMapper;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.ViewModels;

namespace FarmerAndPartnersWebApi.Mappings
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyViewModel>();
            CreateMap<CompanyViewModel, Company>()
                .ForMember(dest => dest.ContractStatus, opt => opt.Ignore());
        }
    }
}
