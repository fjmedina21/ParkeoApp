using Microsoft.Extensions.Configuration;
using ParkeoApp.Application.Helpers;
using ParkeoApp.Domain.Entities;
using ParkeoApp.Domain.Enums;
using ParkeoApp.Infrastructure.Data;

namespace ParkeoApp.Application.Services.ReservationStateService
{
	public interface IReservationStateService
	{
		Task<(bool valid, string? msj)> Transition(ParkeoAppContext dbContext, Reservation reservation, ReservationStatus newStatus);
	}

}
