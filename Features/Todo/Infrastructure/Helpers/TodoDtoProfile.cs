using AutoMapper;
using TodoList.Api.Features.Todo.Core.DTOs;
using TodoList.Api.Features.Todo.Core.Entities;

namespace TodoList.Api.Features.Todo.Infrastructure.Helpers;

public class TodoDtoProfile : Profile
{
    public TodoDtoProfile()
    {
        CreateMap<TodoItem, TodoDto>();
        CreateMap<CreateTodoDto, TodoItem>(MemberList.Source);
        CreateMap<UpdateTodoDto, TodoItem>(MemberList.Source);
    }
}
