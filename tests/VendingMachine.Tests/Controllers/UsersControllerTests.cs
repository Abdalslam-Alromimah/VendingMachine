// UsersControllerTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using VendingMachine.Application.Services;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using Xunit;

namespace VendingMachine.Tests.Controllers;

public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsCreated()
    {
        // Arrange
        var userDto = new UserRegistrationDto
        {
            Username = "testuser",
            Password = "Test123!",
            Role = "buyer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", userDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.Role.Should().Be("buyer");
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange - First register a user
        var userDto = new UserRegistrationDto
        {
            Username = "logintest",
            Password = "Test123!",
            Role = "buyer"
        };
        await _client.PostAsJsonAsync("/api/users/register", userDto);

        var loginDto = new UserLoginDto
        {
            Username = "logintest",
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be("logintest");
    }
}

