// ProductCommands.cs
using MediatR;
using VendingMachine.Domain.DTOs;

namespace VendingMachine.Application.Commands;

public record CreateProductCommand(ProductCreateDto Product, string SellerId) : IRequest<ProductDto>;
public record UpdateProductCommand(int Id, ProductUpdateDto Product, string SellerId) : IRequest<ProductDto>;
public record DeleteProductCommand(int Id, string SellerId) : IRequest;

