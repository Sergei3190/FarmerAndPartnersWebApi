using FarmerAndPartnersEF.EF;
using FarmerAndPartnersModels;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmerAndPartnersEF.Datalnitialization
{
    public static class DataInitializer
    {
        private static readonly ILogger _log = LogManager.GetLogger(nameof(DataInitializer));

        public static FarmerAndPartnersContext GetTestContext() => new FarmerAndPartnersContext();

        public static async Task InitializeData(FarmerAndPartnersContext context = null)
        {
            if (context is null)
                context = new FarmerAndPartnersContext();

            var contractStatuses = new List<ContractStatus>
            {
                new ContractStatus { Id = 1, Definition = "Заключен" },
                new ContractStatus { Id = 2, Definition = "Ещё не заключен" },
                new ContractStatus { Id = 3, Definition = "Расторгнут" }
            };

            await context.ContractStatuses.AddRangeAsync(contractStatuses);

            var companies = new List<Company>
            {
                new Company { Id = 1, Name = "Лодочка", ContractStatus = contractStatuses[0] },
                new Company { Id = 2, Name = "Зверушка", ContractStatus = contractStatuses[1] },
                new Company { Id = 3, Name = "Поедатель", ContractStatus = contractStatuses[2] }
            };

            await context.Companies.AddRangeAsync(companies);

            var users = new List<User>
            {
                new User { Id = 1, Name = "Сергей", Login = "Serg_37", Password = "12322w@", Company = companies[0]},
                new User { Id = 2, Name = "Михаил", Login = "Mih_12", Password = "52622w@", Company = companies[1]},
                new User { Id = 3, Name = "Екатерина", Login = "Kat_31", Password = "78822w@", Company = companies[0]},
                new User { Id = 4, Name = "Светлана", Login = "Svet_15", Password = "66822w@", Company = companies[2]},
                new User { Id = 5, Name = "Дмитрий", Login = "Dima_39", Password = "41522w@", Company = companies[2]},
                new User { Id = 6, Name = "Александра", Login = "Sashka_35", Password = "98522w@", Company = companies[1]},
                new User { Id = 7, Name = "Алекс", Login = "Aleks_36", Password = "77892ww@w@", Company = companies[0]},
                new User { Id = 8, Name = "Макс", Login = "Maks_17", Password = "963772ww@w@", Company = companies[1]},
                new User { Id = 9, Name = "Вероника", Login = "Veronika_18+", Password = "96388ww@w@", Company = companies[2]},
                new User { Id = 10, Name = "Влад", Login = "Vlad_1987", Password = "9159763ww@w@", Company = companies[1]},
                new User { Id = 11, Name = "Николай", Login = "Nikolas_36", Password = "968796ww@w@", Company = companies[0]}
            };

            await context.Users.AddRangeAsync(users);

            await context.SaveChangesAsync();
        }

        public static async Task RecreateDatabaseAsync(FarmerAndPartnersContext context = null)
        {
            if (context is null)
                context = new FarmerAndPartnersContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }

        public static async Task ClearDataAsync(FarmerAndPartnersContext context)
        {
            var companyTable = context.GetTableName(typeof(Company));
            var contractStatusTable = context.GetTableName(typeof(ContractStatus));
            await ExecuteDeleteSqlAsync(context, companyTable);
            await ExecuteDeleteSqlAsync(context, contractStatusTable);
            await ResetIdentityAsync(context, companyTable, contractStatusTable);
        }

        private static async Task ExecuteDeleteSqlAsync(FarmerAndPartnersContext context, string tableName)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                var rawSqlString = $"Delete from dbo.{tableName}";

                try
                {
                    await context.Database.ExecuteSqlRawAsync(rawSqlString);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    _log.Error($"Ошибка при выполнении метода ExecuteDeleteSqlAsync{Environment.NewLine}{ex}");
                    await transaction?.RollbackAsync();
                }
            }
        }

        private static async Task ResetIdentityAsync(FarmerAndPartnersContext context, params string[] tables)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var item in tables)
                    {
                        var reseed = $"DBCC CHECKIDENT (\"dbo.{item}\", RESEED, 0)";
                        await context.Database.ExecuteSqlRawAsync(reseed);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    _log.Error($"Ошибка при выполнении метода ResetIdentityAsync{Environment.NewLine}{ex}");
                    await transaction?.RollbackAsync();
                }
            }
        }
    }
}
