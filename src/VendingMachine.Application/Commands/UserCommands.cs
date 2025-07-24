// UserCommands.cs
using MediatR;
using VendingMachine.Domain.DTOs;

namespace VendingMachine.Application.Commands;

public record CreateUserCommand(UserRegistrationDto User) : IRequest<UserDto>;
public record UpdateUserCommand(string Id, UserUpdateDto User, string CurrentUserId) : IRequest<UserDto>;
public record DeleteUserCommand(string Id, string CurrentUserId) : IRequest;

