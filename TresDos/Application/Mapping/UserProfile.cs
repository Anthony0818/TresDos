using AutoMapper;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Feature.Users.Queries;
using TresDos.Core.Entities;
using TresDos.Feature.Users.Commands;

namespace TresDos.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterUserCommand, User>();
            //CreateMap<GetAllUserQuery, User>();
            CreateMap<User, UserDto>();
        }
    }
}
