using AutoMapper;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.Mappings;
using System.Collections.Generic;

namespace FarmerAndPartnersWebApi.Tests.TestData
{
    internal static class ContractStatusTestData
    {
        private static readonly List<ContractStatus> _contractStatuses = new List<ContractStatus>
        {
             new ContractStatus { Id = 1, Definition = "Заключен" },
             new ContractStatus { Id = 2, Definition = "Ещё не заключен" },
             new ContractStatus { Id = 3, Definition = "Расторгнут" }
        };

        internal static List<ContractStatus> ContractStatuses => _contractStatuses;

        internal static IMapper GetMapper() => new MapperConfiguration(cfg => cfg.AddProfile(new ContractStatusProfile())).CreateMapper();
    }
}