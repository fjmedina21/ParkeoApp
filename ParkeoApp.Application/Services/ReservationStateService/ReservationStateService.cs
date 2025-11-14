using Microsoft.Extensions.Configuration;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Domain.Enums;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ReservationStateService
{
	public class ReservationStateService(IConfiguration configuration) : IReservationStateService
	{
		public async Task<(bool valid, string? msj)> ReservationStateTransition(ParkeoAppContext dbContext, Reservation reservation, ReservationStatus newStatus)
		{
			if (!IsValidTransition(reservation.Status, newStatus))
				return (false, $"Invalid state transition. Current status: {reservation.Status}, attempted new status: {newStatus}");

			reservation.Status = newStatus.ToString();
			reservation.ParkingSpot.Status = MapSpotStatus(newStatus).ToString();
			await dbContext.SaveChangesAsync();

			(string action, string subject) = GetNotificationForTransition(newStatus);

			await Utils.SendReservationEmailNotification(
				reservation!,
				action.ToLower(),
				subject,
				configuration
			);

			return (true, null);
		}

		private static bool IsValidTransition(string currentStatus, ReservationStatus newStatus)
		{
			if (!Enum.TryParse(currentStatus.ToLower(), ignoreCase: true, out ReservationStatus current))
				return false; // evita excepciones si el string no coincide con el enum

			return current switch
			{
				ReservationStatus.Completed => false,
				ReservationStatus.Cancelled => false,
				ReservationStatus.Reserved => newStatus is ReservationStatus.Active or ReservationStatus.Cancelled,
				ReservationStatus.Active => newStatus is ReservationStatus.Completed,
				_ => false
			};
		}

		private static SpotStatus MapSpotStatus(ReservationStatus status)
		{
			return status switch
			{
				ReservationStatus.Completed => SpotStatus.Available,
				ReservationStatus.Cancelled => SpotStatus.Available,
				ReservationStatus.Active => SpotStatus.Occupied,
				_ => SpotStatus.Available
			};
		}
		private static (string action, string subject) GetNotificationForTransition(ReservationStatus status)
		{
			return status switch
			{
				ReservationStatus.Active => ("Checked In", "ParkeoApp: Checked In Reservation"),
				ReservationStatus.Completed => ("Checked Out", "ParkeoApp: Checked Out Reservation"),
				ReservationStatus.Cancelled => ("Cancelled", "ParkeoApp: Cancelled Reservation"),
				_ => ("Modified", "ParkeoApp: Modified Reservation")
			};
		}
	}
}
