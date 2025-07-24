// IAuthService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VendingMachine.Application.Services;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;
using VendingMachine.Domain.ValueObjects;

namespace VendingMachine.Application.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(UserLoginDto loginDto);
    string GenerateJwtToken(string userId, string username, string role);
}