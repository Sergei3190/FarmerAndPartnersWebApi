using FarmerAndPartnersEF.EF;
using FarmerAndPartnersModels;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmerAndPartnersEF.Repository
{
    public class Repository : IAsyncDisposable, IAsyncRepository
    {
        private readonly FarmerAndPartnersContext _db;
        private readonly ILogger _log;

        public Repository() : this(new FarmerAndPartnersContext(), LogManager.GetLogger(nameof(Repository))) { }
        public Repository(ILogger log) : this(new FarmerAndPartnersContext(), log) { }
        public Repository(FarmerAndPartnersContext context, ILogger log)
        {
            _db = context;
            _log = log;
        }

        protected FarmerAndPartnersContext Context => _db;

        public async ValueTask DisposeAsync() => await _db.DisposeAsync();

        public async Task<int> AddCompanyAsync(Company company)
        {
            await _db.Companies.AddAsync(company);
            return await SaveChangesAsync();
        }

        public async Task<int> AddUserAsync(User user)
        {
            await _db.Users.AddAsync(user);
            return await SaveChangesAsync();
        }

        public async Task<int> DeleteCompanyAsync(Company company)
        {
            _db.Companies.Remove(company);
            return await SaveChangesAsync();
        }

        public async Task<int> DeleteUserAsync(User user)
        {
            _db.Users.Remove(user);
            return await SaveChangesAsync();
        }

        public async Task<int> UpdateCompanyAsync(Company company)
        {
            _db.Companies.Update(company);
            return await SaveChangesAsync();
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            _db.Users.Update(user);
            return await SaveChangesAsync();
        }

        public async Task<bool> FindNameCompanyAsync(string nameCompany) => await _db.Companies.AnyAsync(c => c.Name == nameCompany);
        public async Task<bool> FindLoginUserAsync(string loginUser) => await _db.Users.AnyAsync(u => u.Login == loginUser);
        public async Task<int> CheckNameCompanyAsync(string nameCompany) => await _db.Companies.Where(c => c.Name == nameCompany).Select(c => c.Id).FirstOrDefaultAsync();
        public async Task<int> CheckLoginUserAsync(string loginUser) => await _db.Users.Where(u => u.Login == loginUser).Select(u => u.Id).FirstOrDefaultAsync();
        public async Task<Company> GetCompanyAsync(int id) => await _db.Companies.Include(c => c.ContractStatus).Where(c => c.Id == id).FirstOrDefaultAsync();
        public async Task<User> GetUserAsync(int id) => await _db.Users.Include(u => u.Company).ThenInclude(c => c.ContractStatus).Where(u => u.Id == id).FirstOrDefaultAsync();
        public async Task<ContractStatus> GetContractStatusAsync(int id) => await _db.ContractStatuses.Where(c => c.Id == id).FirstOrDefaultAsync();
        public async Task<int> GetIdLastUserAsync()
        {
            var lastUser = await _db.Users.OrderBy(u => u.Id).LastAsync();
            return lastUser.Id;
        }

        public async Task<int> GetIdLastCompanyAsync()
        {
            var lastCompany = await _db.Companies.OrderBy(c => c.Id).LastAsync();
            return lastCompany.Id;
        }

        public int GetCompaniesCount() => Context.Companies.Include(c => c.ContractStatus).Include(c => c.Users).Count();
        public int GetUsersCount() => Context.Users.Count();

        public async Task<int> ExecuteQueryAsync(string sql, params object[] sqlParameters)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await Context.Database.ExecuteSqlRawAsync(sql, sqlParameters);
                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    _log.Error($"Ошибка при выполнении метода ExecuteQueryAsync{Environment.NewLine}{ex}");
                    await transaction.RollbackAsync();
                    return -1;
                }
            }
        }

        public async Task<List<Company>> GetCompaniesAsync() => await Context.Companies.Include(c => c.ContractStatus).Include(c => c.Users).ToListAsync();
        public async Task<List<Company>> GetCompaniesWithoutUsersAsync() => await Context.Companies.Include(c => c.ContractStatus).ToListAsync();
        public async Task<List<User>> GetUsersAsync() => await Context.Users.Include(u => u.Company).ThenInclude(c => c.ContractStatus).ToListAsync();
        public async Task<List<ContractStatus>> GetContractStatusesAsync() => await Context.ContractStatuses.ToListAsync();
        public List<ContractStatus> GetContractStatuses() => Context.ContractStatuses.ToList();
        public IEnumerable<Company> GetCompanies() => Context.Companies.Include(c => c.ContractStatus).Include(c => c.Users);
        public IEnumerable<User> GetUsers() => Context.Users.Include(u => u.Company);

        protected async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _log.Error($"Ошибка параллелизма{Environment.NewLine}{ex}");
                return -1;
            }
            catch (DbUpdateException ex)
            {
                _log.Error($"Ошибка при обновлении базы данных{Environment.NewLine}{ex}");
                return -1;
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при выполнении метода SaveChanges{Environment.NewLine}{ex}");
                return -1;
            }
        }
    }
}
