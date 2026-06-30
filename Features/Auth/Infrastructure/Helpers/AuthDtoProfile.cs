using AutoMapper;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.DTOs;

namespace TodoList.Api.Features.Auth.Infrastructure.Helpers;

public class AuthDtoProfile : Profile
{
    public AuthDtoProfile()
    {
        CreateMap<RegisterDto, User>(MemberList.Source)
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower().Trim()));
    }
}
