using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;

namespace FarmerAndPartnersEF.EF
{
    public class FarmerAndPartnerstContextFactory : IDesignTimeDbContextFactory<FarmerAndPartnersContext>
    {
        private readonly ILogger _log = LogManager.GetLogger(nameof(FarmerAndPartnerstContextFactory));
        private IConfiguration _config;

        public FarmerAndPartnersContext CreateDbContext(string[] args)
        {
            var result = SetConfiguration();

            if (!result)
                return null;
#if DEBUG
            var connectionString = _config.GetConnectionString("MSSqlConnection");
#else
            var connectionString = _config.GetConnectionString("SqlExpressConnection");
#endif
            var optionsBuilder = new DbContextOptionsBuilder<FarmerAndPartnersContext>();

            optionsBuilder.UseSqlServer(connectionString, option => option.EnableRetryOnFailure());

            return new FarmerAndPartnersContext(optionsBuilder.Options);
        }

        private bool SetConfiguration()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
                builder.AddJsonFile("Config.Development.json");
#else
                builder.AddJsonFile("Config.Production.json");
#endif
                _config = builder.Build();

                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при создании конфигурации в методе SetConfiguration(){Environment.NewLine}{ex}");
                return false;
            }
        }
    }
}
