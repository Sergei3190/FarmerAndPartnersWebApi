using AutoMapper;
using FarmerAndPartnersModels;
using FarmerAndPartnersWebApi.Mappings;
using FarmerAndPartnersWebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmerAndPartnersWebApi.Tests.TestData
{
    internal static class CompanyTestData
    {
        private static readonly List<Company> _companies = new List<Company>
        {
            new Company { Id = 1, Name = "Лодочка", TimeStamp = new byte[0], ContractStatusId = 1,
                ContractStatus = new ContractStatus { Id = 1, Definition = "Заключен" } },

            new Company { Id = 2, Name = "Зверушка", TimeStamp = new byte[0], ContractStatusId = 2,
                ContractStatus = new ContractStatus { Id = 2, Definition = "Ещё не заключен" } },

            new Company { Id = 3, Name = "Поедатель", TimeStamp = new byte[2] { 1, 0 }, ContractStatusId = 3,
                ContractStatus = new ContractStatus { Id = 3, Definition = "Расторгнут" } }
        };

        private static readonly Company _conflictCompany = new Company { Id = 2, Name = "Зверушка", TimeStamp = new byte[1] { 0 }, ContractStatusId = 2 };
        private static readonly CompanyViewModel _newCompanyViewModel = new CompanyViewModel { Name = "Патриот", ContractStatusId = 2 };

        private static readonly CompanyViewModel _conflictEditCompanyViewModel = new CompanyViewModel
        {
            Id = 2,
            Name = "Зверь",
            TimeStamp = new byte[0],
            ContractStatusId = 1
        };

        private static readonly CompanyViewModel _editCompanyViewModel = new CompanyViewModel
        {
            Id = 3,
            Name = "Пожиратель",
            TimeStamp = new byte[2] { 1, 0 },
            ContractStatusId = 2
        };

        internal static List<Company> Companies => _companies;
        internal static CompanyViewModel NewCompanyViewModel => _newCompanyViewModel;
        internal static CompanyViewModel EditCompanyViewModel => _editCompanyViewModel;
        internal static CompanyViewModel ConflictEditCompanyViewModel => _conflictEditCompanyViewModel;
        internal static (string Name, string Message) CustomError => ("Name", "Компания с таким именем уже существует");
        internal static string BadName => "Зверушка";
        internal static int BadId => 0;
        internal static int GoodId => 1;
        internal static int ConflictId => 2;

        internal static IMapper GetMapper() => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CompanyProfile>();
            cfg.AddProfile<ContractStatusProfile>();
        })
            .CreateMapper();

        internal static Company GetCompany(int id) => _companies.FirstOrDefault(c => c.Id == id);
        internal static bool FindNameCompany(string name) => _companies.Any(c => c.Name == name);
        internal static int GetIdLastCompany() => _companies.LastOrDefault().Id;

        internal static int AddCompany(Company company)
        {
            if (company is null)
                return -1;

            var count = _companies.Count;

            _companies.Add(company);

            if (count != _companies.Count)
                return 1;

            return -1;
        }

        internal static int AddCompanyStatusCode500(Company company) => -1;
        internal static ContractStatus GetContractStatus(int id) => ContractStatusTestData.ContractStatuses.FirstOrDefault(c => c.Id == id);
        internal static int CheckNameCompany(string name) => _companies.Where(c => c.Name == name).Select(c => c.Id).FirstOrDefault();

        internal static int UpdateCompany(Company company)
        {
            var sourceCompany = _companies.FirstOrDefault(c => c.Id == company.Id && c.TimeStamp.SequenceEqual(company.TimeStamp));

            if (sourceCompany is null)
                return -1;

            if (!sourceCompany.Name.Equals(company.Name, StringComparison.OrdinalIgnoreCase))
                sourceCompany.Name = company.Name;

            if (sourceCompany.ContractStatusId != company.ContractStatusId)
                sourceCompany.ContractStatusId = company.ContractStatusId;

            return 1;
        }

        internal static int UpdateConflictCompany(Company company)
        {
            if (_conflictCompany.Id == company.Id && _conflictCompany.TimeStamp != company.TimeStamp)
                return -1;

            return 1;
        }

        internal static int DeleteCompany(Company company)
        {
            if (!_companies.Any(c => c.Id == company.Id && c.TimeStamp == company.TimeStamp))
                return -1;

            var result = _companies.Remove(company);

            return result switch
            {
                false => -1,
                true => 1
            };
        }

        internal static int DeleteConflictCompany(Company company)
        {
            if (_conflictCompany.Id == company.Id && _conflictCompany.TimeStamp != company.TimeStamp)
                return -1;

            return 1;
        }
    }
}
