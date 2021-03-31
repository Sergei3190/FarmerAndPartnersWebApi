using FarmerAndPartnersWebApi.Mappings;
using FarmerAndPartnersWebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;

namespace FarmerAndPartnersWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.Configuration = new NLogLoggingConfiguration(Configuration.GetSection("NLog"));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            var connection = Configuration.GetConnectionString("TestConnection");
#else
            var connection = Configuration.GetConnectionString("ProdConnection");
#endif

            services.AddMvc()
                .AddXmlDataContractSerializerFormatters()
                .AddMvcOptions(options =>
                {
                    options.FormatterMappings.SetMediaTypeMappingForFormat("xml", new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/xml"));
                });

            services.AddRepositoryService();

            services.AddAutoMapper(typeof(CompanyProfile));
            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(ContractStatusProfile));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
