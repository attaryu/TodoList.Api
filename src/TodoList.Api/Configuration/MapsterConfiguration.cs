using Mapster;
using TodoList.Api.Application.DTOs.Auth.Outputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Configuration;

public static class MapsterConfiguration
{
    public static void Configure()
    {
        TypeAdapterConfig<TodoItem, TodoResultDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.IsCompleted, src => src.IsCompleted)
            .Map(dest => dest.CompletedAt, src => src.CompletedAt)
            .Map(dest => dest.CreatedDate, src => src.CreatedDate)
            .Map(dest => dest.UpdatedDate, src => src.UpdatedDate);

        TypeAdapterConfig<User, UserResultDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Fullname, src => src.Fullname)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.IsEmailVerified, src => src.IsEmailVerified);
    }
}
