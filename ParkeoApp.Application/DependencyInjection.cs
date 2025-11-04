using System.Reflection;
using ParkeoApp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkeoApp.Application.Middlewares;
using ParkeoApp.Application.Services.AuthService;
using ParkeoApp.Application.Services.ParkingLotService;
using ParkeoApp.Application.Services.ParkingSpotService;
using ParkeoApp.Application.Services.RoleService;
using ParkeoApp.Application.Services.TenantService;
using ParkeoApp.Application.Services.UserService;

namespace ParkeoApp.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
		{

			services.AddAutoMapper(cfg =>
			{
				cfg.LicenseKey = configuration.GetValue<string>("LuckyPennySoftwareLicenseKey");
				cfg.AddProfile<AutoMappingProfiles>();
			});

			// services.AddScoped<NotificationsService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<ITenantService, TenantService>();
			services.AddScoped<IParkingLotService, ParkingLotService>();
			services.AddScoped<IParkingSpotService, ParkingSpotService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IRoleService, RoleService>();


			services.AddTransient<GlobalErrorHandler>();

			services.AddInfrastructure(configuration);

			return services;
		}
	}
}
