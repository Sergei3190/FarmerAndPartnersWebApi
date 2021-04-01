using AutoMapper;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.ViewModels;

namespace FarmerAndPartnersWebApi.Mappings
{
    public class ContractStatusProfile : Profile
    {
        public ContractStatusProfile()
        {
            CreateMap<ContractStatus, ContractStatusViewModel>();
            CreateMap<ContractStatusViewModel, ContractStatus>();
        }
    }
}
