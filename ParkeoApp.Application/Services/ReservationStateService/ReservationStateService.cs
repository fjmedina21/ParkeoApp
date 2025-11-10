using Microsoft.Extensions.Configuration;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Domain.Enums;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ReservationStateService
{
	public class ReservationStateService(IConfiguration configuration) : IReservationStateService
	{
		public async Task<(bool valid, string? msj)> Transition(ParkeoAppContext dbContext, Reservation reservation, ReservationStatus newStatus)
		{
			if (!IsValidTransition(reservation.Status, newStatus))
				return (false, "Transición de estado inválida.");

			reservation.Status = newStatus.ToString();
			reservation.Spot.Status = MapSpotStatus(newStatus).ToString();
			await dbContext.SaveChangesAsync();

			(string action, string subject) = GetNotificationForTransition(newStatus);

			Utils.SendReservationEmailNotification(
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
				ReservationStatus.Active => newStatus is ReservationStatus.Completed ,
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
				ReservationStatus.Active => ("Check In", "ParkeoApp: Tu reserva está activa"),
				ReservationStatus.Completed => ("Check Out", "ParkeoApp: Tu reserva ha finalizado"),
				ReservationStatus.Cancelled => ("Cancelada", "ParkeoApp: Tu reserva fue cancelada"),
				_ => ("Actualizada", "ParkeoApp: Actualización de reserva")
			};
		}
	}
}
