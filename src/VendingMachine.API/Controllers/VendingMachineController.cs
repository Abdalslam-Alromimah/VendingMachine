// VendingMachineController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendingMachine.Application.Commands;
using VendingMachine.Domain.DTOs;

namespace VendingMachine.API.Controllers;

[ApiController]
[Route("api/vending")]
[Authorize(Roles = "buyer")]
public class VendingMachineController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendingMachineController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("deposit")]
    public async Task<ActionResult<DepositResponseDto>> Deposit([FromBody] DepositDto depositDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var command = new DepositCommand(userId, depositDto);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("buy")]
    public async Task<ActionResult<BuyResponseDto>> Buy([FromBody] BuyRequestDto buyRequest)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var command = new BuyCommand(userId, buyRequest);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var command = new ResetDepositCommand(userId);
        await _mediator.Send(command);
        return Ok(new { message = "Deposit reset successfully" });
    }
}