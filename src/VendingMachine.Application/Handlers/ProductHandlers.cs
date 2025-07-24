
// ProductHandlers.cs
using MediatR;
using Microsoft.Extensions.Logging;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Queries;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;

namespace VendingMachine.Application.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductRepository productRepository,
        IUserRepository userRepository,
        ILogger<CreateProductHandler> logger)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product: {ProductName} by seller: {SellerId}",
            request.Product.ProductName, request.SellerId);

        var seller = await _userRepository.GetByIdAsync(request.SellerId);
        if (seller == null)
        {
            throw new UserNotFoundException(request.SellerId);
        }

        var product = new Product
        {
            ProductName = request.Product.ProductName,
            AmountAvailable = request.Product.AmountAvailable,
            Cost = request.Product.Cost,
            SellerId = request.SellerId
        };

        var createdProduct = await _productRepository.CreateAsync(product);

        _logger.LogInformation("Product created successfully with ID: {ProductId}", createdProduct.Id);

        return new ProductDto
        {
            Id = createdProduct.Id,
            ProductName = createdProduct.ProductName,
            AmountAvailable = createdProduct.AmountAvailable,
            Cost = createdProduct.Cost,
            SellerId = createdProduct.SellerId,
            SellerUsername = seller.UserName!
        };
    }
}

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository productRepository,
        IUserRepository userRepository,
        ILogger<UpdateProductHandler> logger)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        if (product.SellerId != request.SellerId)
        {
            throw new UnauthorizedException("You can only update your own products");
        }

        if (!string.IsNullOrWhiteSpace(request.Product.ProductName))
        {
            product.ProductName = request.Product.ProductName;
        }

        if (request.Product.AmountAvailable.HasValue)
        {
            product.AmountAvailable = request.Product.AmountAvailable.Value;
        }

        if (request.Product.Cost.HasValue)
        {
            product.Cost = request.Product.Cost.Value;
        }

        await _productRepository.UpdateAsync(product);

        var seller = await _userRepository.GetByIdAsync(product.SellerId);

        _logger.LogInformation("Product updated successfully: {ProductId}", product.Id);

        return new ProductDto
        {
            Id = product.Id,
            ProductName = product.ProductName,
            AmountAvailable = product.AmountAvailable,
            Cost = product.Cost,
            SellerId = product.SellerId,
            SellerUsername = seller?.UserName ?? "Unknown"
        };
    }
}

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(IProductRepository productRepository, ILogger<DeleteProductHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new ProductNotFoundException(request.Id);
        }

        if (product.SellerId != request.SellerId)
        {
            throw new UnauthorizedException("You can only delete your own products");
        }

        await _productRepository.DeleteAsync(request.Id);
        _logger.LogInformation("Product deleted successfully: {ProductId}", request.Id);
    }
}

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            ProductName = product.ProductName,
            AmountAvailable = product.AmountAvailable,
            Cost = product.Cost,
            SellerId = product.SellerId,
            SellerUsername = product.Seller?.UserName ?? "Unknown"
        };
    }
}

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            ProductName = p.ProductName,
            AmountAvailable = p.AmountAvailable,
            Cost = p.Cost,
            SellerId = p.SellerId,
            SellerUsername = p.Seller?.UserName ?? "Unknown"
        });
    }
}

public class GetProductsBySellerIdHandler : IRequestHandler<GetProductsBySellerIdQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsBySellerIdHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsBySellerIdQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetBySellerIdAsync(request.SellerId);

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            ProductName = p.ProductName,
            AmountAvailable = p.AmountAvailable,
            Cost = p.Cost,
            SellerId = p.SellerId,
            SellerUsername = p.Seller?.UserName ?? "Unknown"
        });
    }
}