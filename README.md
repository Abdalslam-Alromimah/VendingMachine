# Vending Machine API

A comprehensive REST API for a vending machine system built with .NET 8, implementing Clean Architecture with CQRS pattern, Docker support, and PostgreSQL database.

## 🚀 Features

- Complete CRUD Operations for users and products
- Role-based Authentication (buyer/seller) with JWT tokens
- Vending Machine Operations: deposit, buy, reset
- Coin System: Supports 5, 10, 20, 50, and 100 cent coins
- Change Calculation: Optimal change distribution
- CQRS Pattern with MediatR
- Clean Architecture with separated layers
- Docker Support with PostgreSQL
- Comprehensive Testing (Unit & Integration tests)
- Exception Handling with custom middleware
- Logging with Serilog
- API Documentation with Swagger/OpenAPI

## 🏗️ Architecture

```
┌─────────────────┐
│   API Layer     │ ← Controllers, Middleware
├─────────────────┤
│ Application     │ ← Commands, Queries, Handlers, Services
├─────────────────┤
│   Domain        │ ← Entities, DTOs, Interfaces, Exceptions
├─────────────────┤
│ Infrastructure  │ ← Database, Repositories, External Services
└─────────────────┘
```

## 🛠️ Technology Stack

- .NET 8 - Framework
- ASP.NET Core - Web API
- Entity Framework Core - ORM
- PostgreSQL - Database
- JWT - Authentication
- MediatR - CQRS implementation
- Serilog - Logging
- Docker - Containerization
- xUnit - Testing framework
- FluentAssertions - Test assertions
- Swagger - API documentation

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose
- Git

### Running with Docker (Recommended)

1. Clone the repository
   ```bash
   git clone <repository-url>
   cd VendingMachine
   ```

2. Start the application
   ```bash
   docker-compose up --build
   ```

3. Access the API
   - API: http://localhost:8080
   - Swagger UI: http://localhost:8080/swagger

### Running Locally

1. Install PostgreSQL and create database
   ```sql
   CREATE DATABASE vendingmachine;
   ```

2. Update connection string in `appsettings.json`

3. Install dependencies and run
   ```bash
   dotnet restore
   dotnet ef database update --project src/VendingMachine.Infrastructure --startup-project src/VendingMachine.API
   dotnet run --project src/VendingMachine.API
   ```

## 📝 API Endpoints

### Authentication
```http
POST /api/users/register    # Register new user
POST /api/users/login       # Login user
```

### Users (CRUD)
```http
GET    /api/users/{id}      # Get user by ID (authenticated)
PUT    /api/users/{id}      # Update user (owner only)
DELETE /api/users/{id}      # Delete user (owner only)
```

### Products
```http
GET    /api/products           # Get all products (public)
GET    /api/products/{id}      # Get product by ID (public)
POST   /api/products           # Create product (seller only)
PUT    /api/products/{id}      # Update product (owner only)
DELETE /api/products/{id}      # Delete product (owner only)
GET    /api/products/seller/{sellerId} # Get products by seller
```

### Vending Machine (Buyer only)
```http
POST /api/vending/deposit    # Deposit coins
POST /api/vending/buy        # Buy products
POST /api/vending/reset      # Reset deposit
```

## 💡 Usage Examples

### 1. Register a User
```bash
curl -X POST "http://localhost:8080/api/users/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "buyer1",
    "password": "Buyer123!",
    "role": "buyer"
  }'
```

### 2. Login
```bash
curl -X POST "http://localhost:8080/api/users/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "buyer1",
    "password": "Buyer123!"
  }'
```

### 3. Create Product (Seller)
```bash
curl -X POST "http://localhost:8080/api/products" \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "productName": "Coca Cola",
    "amountAvailable": 10,
    "cost": 100
  }'
```

### 4. Deposit Coins (Buyer)
```bash
curl -X POST "http://localhost:8080/api/vending/deposit" \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "coins": {
      "100": 2,
      "50": 1
    }
  }'
```

### 5. Buy Product (Buyer)
```bash
curl -X POST "http://localhost:8080/api/vending/buy" \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": 1,
    "amount": 1
  }'
```

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Category
```bash
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

### Test Coverage
The solution includes comprehensive tests:
- Unit Tests: Services, handlers, utilities
- Integration Tests: Controllers, database operations
- End-to-End Tests: Complete user workflows

## 🔒 Security Features

- JWT Authentication with configurable expiration
- Role-based Authorization (buyer/seller)
- Password Hashing with BCrypt
- Input Validation and sanitization
- SQL Injection Protection via Entity Framework
- CORS Configuration for cross-origin requests

## 🏭 Production Readiness

### Logging
- Structured logging with Serilog
- Console and file outputs
- Configurable log levels
- Request/response logging

### Exception Handling
- Global exception middleware
- Domain-specific exceptions
- Consistent error responses
- Error tracking and monitoring

### Database
- Connection pooling
- Migration support
- Seed data for testing
- Index optimization

### Performance
- Async/await throughout
- Efficient queries with EF Core
- Caching considerations
- Resource cleanup

## 🐳 Docker Configuration

### Services
- API: .NET 8 web application
- Database: PostgreSQL 15

### Environment Variables
```yaml
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=postgres;Database=vendingmachine;Username=postgres;Password=postgres123
```

## 📊 Database Schema

### Users (ApplicationUser)
- Id (string, PK)
- Username (string, unique)
- Email (string)
- Deposit (int, cents)
- Password (hashed)

### Products
- Id (int, PK)
- ProductName (string)
- AmountAvailable (int)
- Cost (int, cents)
- SellerId (string, FK)

### Roles
- buyer: Can deposit coins and buy products
- seller: Can manage products

## 🔧 Configuration

### JWT Settings
```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "VendingMachineAPI",
    "Audience": "VendingMachineUsers",
    "ExpireMinutes": 60
  }
}
```

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=vendingmachine;Username=postgres;Password=yourpassword"
  }
}
```

## 🚨 Error Handling

The API returns consistent error responses:

```json
{
  "statusCode": 400,
  "message": "Invalid coin denomination: 15. Valid denominations are: 5, 10, 20, 50, 100 cents.",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Common Error Codes
- 400: Bad Request (validation errors, business rule violations)
- 401: Unauthorized (invalid credentials, missing token)
- 403: Forbidden (insufficient permissions)
- 404: Not Found (resource doesn't exist)
- 500: Internal Server Error (unexpected errors)

## 📈 Monitoring and Observability

### Logs
- Application logs in `/logs` directory
- Structured JSON format
- Different log levels (Debug, Info, Warning, Error)
- Request correlation IDs

### Health Checks
- Database connectivity
- Application status
- Available at `/health` endpoint

## 🔄 Development Workflow

### Database Migrations
```bash
# Add migration
dotnet ef migrations add <MigrationName> --project src/VendingMachine.Infrastructure --startup-project src/VendingMachine.API

# Update database
dotnet ef database update --project src/VendingMachine.Infrastructure --startup-project src/VendingMachine.API
```

### Code Quality
- Clean Architecture principles
- SOLID principles
- Dependency Injection
- Separation of Concerns
- Domain-Driven Design concepts

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📋 Pre-seeded Data

The application comes with sample data:
- Seller: username: `seller1`, password: `Seller123!`
- Buyer: username: `buyer1`, password: `Buyer123!`
- Products: Coca Cola, Pepsi, Water, Snickers, Chips

## 🔮 Future Enhancements

- [ ] Product categories and filtering
- [ ] Purchase history and analytics
- [ ] Multiple vending machines support
- [ ] Real-time notifications
- [ ] Admin dashboard
- [ ] Payment gateway integration
- [ ] Inventory management alerts
- [ ] Multi-language support

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For questions and support:
- Check the documentation
- Review test cases for usage examples
- Open an issue for bugs or feature requests

---

Generative AI Usage: This solution was developed with assistance from Claude AI to implement Clean Architecture patterns, CQRS design, comprehensive testing, and production-ready features.

*Built with ❤️ using .NET 8 and Clean Architecture*