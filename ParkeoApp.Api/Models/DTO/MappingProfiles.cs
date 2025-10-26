using ParkeoApp.Api.Models.Entities;
using AutoMapper;

namespace ParkeoApp.Api.Models.DTO
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<user, GetUser>();
            CreateMap<AddUser, user>();

            CreateMap<tenant, GetTenant>();
            CreateMap<AddTenant, tenant>();

        }
    }
}
