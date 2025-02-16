using AutoMapper;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;

namespace SocialVN.API.Mappings
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles() {

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AddUserRequestDto, User>().ReverseMap();
        }

    }
        
}
