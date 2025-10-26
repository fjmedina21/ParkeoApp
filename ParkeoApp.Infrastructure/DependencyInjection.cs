using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ParkeoAppContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // services.AddSignalR();


            return services;
        }
    }
}