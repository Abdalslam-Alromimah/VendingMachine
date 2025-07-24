// ProductsController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Queries;
using VendingMachine.Domain.DTOs;

namespace VendingMachine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "seller")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductCreateDto productDto)
    {
        var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var command = new CreateProductCommand(productDto, sellerId);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "seller")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] ProductUpdateDto productDto)
    {
        var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var command = new UpdateProductCommand(id, productDto, sellerId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "seller")]
    public async Task<IActionResult> Delete(int id)
    {
        var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var command = new DeleteProductCommand(id, sellerId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("seller/{sellerId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetBySellerId(string sellerId)
    {
        var query = new GetProductsBySellerIdQuery(sellerId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

