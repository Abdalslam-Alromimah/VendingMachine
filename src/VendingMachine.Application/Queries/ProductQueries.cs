// ProductQueries.cs
using MediatR;
using VendingMachine.Domain.DTOs;

namespace VendingMachine.Application.Queries;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
public record GetProductsBySellerIdQuery(string SellerId) : IRequest<IEnumerable<ProductDto>>;