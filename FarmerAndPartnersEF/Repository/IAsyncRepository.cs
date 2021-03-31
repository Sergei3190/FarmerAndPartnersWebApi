using FarmerAndPartnersModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmerAndPartnersEF.Repository
{
    public interface IAsyncRepository
    {
        Task<int> AddCompanyAsync(Company company);
        Task<int> DeleteCompanyAsync(Company company);
        Task<int> UpdateCompanyAsync(Company company);
        Task<bool> FindNameCompanyAsync(string nameCompany);
        Task<int> CheckNameCompanyAsync(string nameCompany);
        Task<int> GetIdLastCompanyAsync();
        Task<Company> GetCompanyAsync(int id);
        Task<int> AddUserAsync(User user);
        Task<int> DeleteUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
        Task<bool> FindLoginUserAsync(string loginUser);
        Task<int> CheckLoginUserAsync(string loginUser);
        Task<int> GetIdLastUserAsync();
        Task<User> GetUserAsync(int id);
        Task<ContractStatus> GetContractStatusAsync(int id);
        Task<int> ExecuteQueryAsync(string sql, params object[] sqlParameters);
        Task<List<Company>> GetCompaniesAsync();
        Task<List<Company>> GetCompaniesWithoutUsersAsync();
        Task<List<User>> GetUsersAsync();
        Task<List<ContractStatus>> GetContractStatusesAsync();
        List<ContractStatus> GetContractStatuses();
        IEnumerable<Company> GetCompanies();
        IEnumerable<User> GetUsers();
        int GetCompaniesCount();
        int GetUsersCount();
    }
}
