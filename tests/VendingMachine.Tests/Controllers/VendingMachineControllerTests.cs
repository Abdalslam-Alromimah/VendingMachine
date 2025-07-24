// VendingMachineControllerTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using VendingMachine.Domain.DTOs;
using Xunit;

namespace VendingMachine.Tests.Controllers;

public class VendingMachineControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public VendingMachineControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<string> GetBuyerTokenAsync()
    {
        // Register and login as buyer
        var userDto = new UserRegistrationDto
        {
            Username = $"buyer_{Guid.NewGuid()}",
            Password = "Test123!",
            Role = "buyer"
        };
        await _client.PostAsJsonAsync("/api/users/register", userDto);

        var loginDto = new UserLoginDto
        {
            Username = userDto.Username,
            Password = "Test123!"
        };

        var response = await _client.PostAsJsonAsync("/api/users/login", loginDto);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return result!.Token;
    }

    [Fact]
    public async Task Deposit_ValidCoins_ReturnsSuccess()
    {
        // Arrange
        var token = await GetBuyerTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var depositDto = new DepositDto
        {
            Coins = new Dictionary<int, int> { { 100, 2 }, { 50, 1 } }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vending/deposit", depositDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DepositResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        result.Should().NotBeNull();
        result!.NewDeposit.Should().Be(250);
    }
}

