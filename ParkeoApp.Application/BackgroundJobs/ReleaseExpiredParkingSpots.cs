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
	public class ReleaseExpiredParkingSpots(ILogger logger, IServiceProvider serviceProvider) : BackgroundService
	{
		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			logger.Warning("ReleaseExpiredParkingSpots background job started.");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using IServiceScope scope = serviceProvider.CreateScope();
					ParkeoAppContext db = scope.ServiceProvider.GetRequiredService<ParkeoAppContext>();
					IReservationStateService stateService = scope.ServiceProvider.GetRequiredService<IReservationStateService>();

					await CompleteExpiredReservations(db, stateService);
				}
				catch (Exception ex)
				{
					logger.Error(ex, "An error occurred while releasing expired parking spots.");
				}

				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}

			logger.Warning("ReleaseExpiredParkingSpots background job stopped.");
		}

		private async Task CompleteExpiredReservations(
			ParkeoAppContext dbContext,
			IReservationStateService stateService
		)
		{
			DateTime now = DateTime.Now;

			var expiredReservations = await dbContext.Reservations
				.Include(r => r.ParkingSpot).ThenInclude(e=>e.ParkingLot)
				.Include(r => r.User)
				.Where(r =>
					!r.DeletedAt.HasValue &&
					r.EndAt <= now &&
					r.Status == nameof(ReservationStatus.Active)
				).ToListAsync();

			if (!expiredReservations.Any()) return;

			foreach (Reservation reservation in expiredReservations)
			{
				try
				{
					await stateService.ReservationStateTransition(dbContext, reservation, ReservationStatus.Completed);
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Error completing reservation {reservation.Code}");
				}
			}
		}
	}
}
