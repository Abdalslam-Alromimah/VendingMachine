// UserQueries.cs
using MediatR;
using VendingMachine.Domain.DTOs;

namespace VendingMachine.Application.Queries;

public record GetUserByIdQuery(string Id) : IRequest<UserDto?>;
public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;

