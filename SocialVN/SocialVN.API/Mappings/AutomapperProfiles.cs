using AutoMapper;
using SocialVN.API.Models.Domain;
using SocialVN.API.Models.DTO;
using System.Xml.Linq;

namespace SocialVN.API.Mappings
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles() {

            
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<AddUserRequestDto, ApplicationUser>().ReverseMap();
            CreateMap<UpdateUserRequestDto, ApplicationUser>().ReverseMap();

            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<AddCommentRequestDto, Comment>().ReverseMap();
            CreateMap<UpdateCommentRequestDto, Comment>().ReverseMap();

            CreateMap<Like, LikeDto>().ReverseMap();
            CreateMap<AddLikeRequestDto, Like>().ReverseMap();


            CreateMap<Post, PostDto>().ReverseMap();
            CreateMap<AddPostRequestDto, Post>().ReverseMap();
            CreateMap<UpdatePostRequestDto, Post>().ReverseMap();

            CreateMap<Friendship, FriendshipDto>().ReverseMap();
            CreateMap<AddFriendshipRequestDto, Friendship>().ReverseMap();
        }

    }
        
}
