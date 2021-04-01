using FarmerAndPartnersEF.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FarmerAndPartnersWebApi.Services
{
    public static class ServiceProviderExtensions
    {
        public static void AddRepositoryService(this IServiceCollection service)
        {
            if (service is null)
                throw new ArgumentNullException(nameof(service));

            service.AddScoped<IAsyncRepository, Repository>();
        }
    }
}
