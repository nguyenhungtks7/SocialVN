using AutoMapper;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using System.Xml.Linq;

namespace SocialVN.API.Mappings
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles() {

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AddUserRequestDto, User>().ReverseMap();
            CreateMap<UpdateUserRequestDto, User>().ReverseMap();

            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<AddCommentRequestDto, Comment>().ReverseMap();
            CreateMap<UpdateCommentRequestDto, Comment>().ReverseMap();

            CreateMap<Like, LikeDto>().ReverseMap();
            CreateMap<AddLikeRequestDto, Like>().ReverseMap();
        }

    }
        
}
