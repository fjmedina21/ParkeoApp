using AutoMapper;
using ParkeoApp.Domain.DTO;
using ParkeoApp.Domain.Entities;

namespace ParkeoApp.Application
{
	public class AutoMappingProfiles : Profile
	{
		public AutoMappingProfiles()
		{
			CreateMap<user, GetUser>();
			CreateMap<AddUser, user>();

			CreateMap<tenant, GetTenant>();
			CreateMap<AddTenant, tenant>();
		}
	}
}
