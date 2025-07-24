// UserHandlers.cs
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Queries;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;

namespace VendingMachine.Application.Handlers;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(
        IUserRepository userRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<CreateUserHandler> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user with username: {Username}", request.User.Username);

        var existingUser = await _userRepository.GetByUsernameAsync(request.User.Username);
        if (existingUser != null)
        {
            throw new DuplicateUsernameException(request.User.Username);
        }

        var user = new ApplicationUser
        {
            UserName = request.User.Username,
            Email = $"{request.User.Username}@example.com",
            Deposit = 0
        };

        await _userRepository.CreateAsync(user, request.User.Password);
        await _userManager.AddToRoleAsync(user, request.User.Role);

        _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Deposit = user.Deposit,
            Role = request.User.Role
        };
    }
}

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(
        IUserRepository userRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<UpdateUserHandler> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Id != request.CurrentUserId)
        {
            throw new UnauthorizedException("You can only update your own profile");
        }

        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null)
        {
            throw new UserNotFoundException(request.Id);
        }

        if (!string.IsNullOrWhiteSpace(request.User.Username))
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.User.Username);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new DuplicateUsernameException(request.User.Username);
            }
            user.UserName = request.User.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.User.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, request.User.Password);
        }
        else
        {
            await _userRepository.UpdateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "buyer";

        _logger.LogInformation("User updated successfully: {UserId}", user.Id);

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Deposit = user.Deposit,
            Role = role
        };
    }
}

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandler(IUserRepository userRepository, ILogger<DeleteUserHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Id != request.CurrentUserId)
        {
            throw new UnauthorizedException("You can only delete your own account");
        }

        if (!await _userRepository.ExistsAsync(request.Id))
        {
            throw new UserNotFoundException(request.Id);
        }

        await _userRepository.DeleteAsync(request.Id);
        _logger.LogInformation("User deleted successfully: {UserId}", request.Id);
    }
}

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdHandler(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "buyer";

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Deposit = user.Deposit,
            Role = role
        };
    }
}

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllUsersHandler(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "buyer";

            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Deposit = user.Deposit,
                Role = role
            });
        }

        return userDtos;
    }
}
