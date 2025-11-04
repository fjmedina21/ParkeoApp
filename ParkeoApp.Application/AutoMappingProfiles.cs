using AutoMapper;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;

namespace ParkeoApp.Application
{
	public class AutoMappingProfiles : Profile
	{
		public AutoMappingProfiles()
		{
			CreateMap<User, GetUser>();
			CreateMap<User, GetUserWNRef>();
			CreateMap<AddUser, User>();

			CreateMap<Tenant, GetTenant>();
			CreateMap<AddTenant, Tenant>();

			CreateMap<Reservation, GetReservation>();
			CreateMap<Reservation, GetReservationWNRef>();
			CreateMap<AddReservation, Reservation>();

			CreateMap<ParkingLot, GetParkingLot>();
			CreateMap<ParkingLot, GetParkingLotWNRef>();
			CreateMap<AddParkingLot, ParkingLot>();

			CreateMap<ParkingSpot, GetParkingSpot>();
			CreateMap<ParkingSpot, GetParkingSpotWNRef>();
			CreateMap<AddParkingSpot, ParkingSpot>();

			CreateMap<Role, GetRole>();
			CreateMap<AddRole, Role>();

			CreateMap<Permission, GetPermission>();
			CreateMap<AddPermission, Permission>();


		}
	}
}
