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
			CreateMap<User, GetUserWnRef>();
			CreateMap<AddUser, User>();

			CreateMap<Tenant, GetTenant>();
			CreateMap<AddTenant, Tenant>();

			CreateMap<Reservation, GetReservation>();
			CreateMap<AddReservation, Reservation>();

			CreateMap<ParkingLot, GetParkingLot>()
				.ForMember(dest => dest.Available, opt => opt.MapFrom(src => src.ParkingSpots.Count(e => e.Status.ToLower().Equals("available"))))
				.ForMember(dest => dest.Occupied, opt => opt.MapFrom(src => src.ParkingSpots.Count(e => e.Status.ToLower().Equals("occupied"))))
				.ForMember(dest => dest.Maintenance, opt => opt.MapFrom(src => src.ParkingSpots.Count(e => e.Status.ToLower().Equals("maintenance"))));
			CreateMap<AddParkingLot, ParkingLot>();

			CreateMap<ParkingSpot, GetParkingSpot>();
			CreateMap<AddParkingSpot, ParkingSpot>();

			CreateMap<Role, GetRole>();
			CreateMap<AddRole, Role>();

			CreateMap<Permission, GetPermission>();
			CreateMap<AddPermission, Permission>();
		}
	}
}
