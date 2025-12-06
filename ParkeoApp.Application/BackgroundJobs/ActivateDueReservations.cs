using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ParkeoApp.Infrastructure.Data;
using ParkeoApp.Application.Services.ReservationStateService;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Domain.Enums;

namespace ParkeoApp.Application.BackgroundJobs
{
	public class ActivateDueReservations(ILogger logger, IServiceProvider serviceProvider) : BackgroundService
	{
		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			logger.Information("ActivateDueReservationsJob background job started.");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using IServiceScope scope = serviceProvider.CreateScope();
					ParkeoAppContext db = scope.ServiceProvider.GetRequiredService<ParkeoAppContext>();
					IReservationStateService stateService = scope.ServiceProvider.GetRequiredService<IReservationStateService>();

					await ActivateReservations(db, stateService);
				}
				catch (Exception ex)
				{
					logger.Error(ex, "An error occurred while releasing expired parking spots.");
				}

				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}

			logger.Information("ActivateDueReservationsJob background job stopped.");
		}

		private async Task ActivateReservations(
			ParkeoAppContext dbContext,
			IReservationStateService stateService
		)
		{
			DateTime now = DateTime.Now;

			var activeReservations = await dbContext.Reservations
				.Include(r => r.ParkingSpot).ThenInclude(e=>e.ParkingLot)
				.Include(r => r.User)

				.Where(r =>
					!r.DeletedAt.HasValue &&
					r.StartAt <= now &&
					r.Status == nameof(ReservationStatus.Reserved)
				).ToListAsync();

			if (!activeReservations.Any()) return;

			foreach (Reservation reservation in activeReservations)
			{
				try
				{
					await stateService.ReservationStateTransition(dbContext, reservation, ReservationStatus.Active);
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Error activating reservation {reservation.Code}");
				}
			}
		}
	}
}
