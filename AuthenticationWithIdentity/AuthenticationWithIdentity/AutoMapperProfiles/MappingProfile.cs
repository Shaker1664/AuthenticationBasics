using AuthenticationWithIdentity.DataTransferObjects;
using AuthenticationWithIdentity.Entities;
using AutoMapper;

namespace AuthenticationWithIdentity.AutoMapperProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<UserForRegistration, User>();
        }
    }
}
