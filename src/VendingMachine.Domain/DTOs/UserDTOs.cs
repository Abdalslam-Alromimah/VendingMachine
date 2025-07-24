// UserDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Domain.DTOs;

public class UserRegistrationDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(buyer|seller)$", ErrorMessage = "Role must be either 'buyer' or 'seller'")]
    public string Role { get; set; } = string.Empty;
}

public class UserLoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Deposit { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class UserUpdateDto
{
    [StringLength(50, MinimumLength = 3)]
    public string? Username { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

