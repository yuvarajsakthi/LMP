# Development Guidelines

## Code Quality Standards

### Formatting and Structure
- **Namespace Organization**: Use hierarchical namespace structure matching folder organization (e.g., `Kanini.LMP.Application.Services.Implementations`)
- **File Organization**: One primary class per file, with filename matching the class name
- **Line Breaks**: Use Windows-style line endings (CRLR - `\r\n`)
- **Indentation**: Use consistent indentation (4 spaces for C#, 2 spaces for TypeScript)
- **Nullable Reference Types**: Enable nullable reference types in C# projects (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enable implicit usings in .NET projects (`<ImplicitUsings>enable</ImplicitUsings>`)

### Naming Conventions

#### C# Backend
- **Classes**: PascalCase (e.g., `EmiCalculatorService`, `CreditScoreService`)
- **Interfaces**: PascalCase with 'I' prefix (e.g., `IEmiCalculatorService`, `IUnitOfWork`)
- **Methods**: PascalCase with descriptive action verbs (e.g., `CalculateEmiAsync`, `GetCreditScoreAsync`)
- **Private Methods**: PascalCase (e.g., `MapToDto`, `GenerateMockCreditScore`)
- **Parameters**: camelCase (e.g., `principalAmount`, `interestRate`, `termMonths`)
- **Local Variables**: camelCase (e.g., `monthlyRate`, `emi`, `totalRepayment`)
- **Constants**: PascalCase in static classes (e.g., `ApplicationConstants.Messages.Success`)
- **Async Methods**: Suffix with 'Async' (e.g., `CalculateEmiAsync`, `CreateEmiPlanAsync`)

#### TypeScript Frontend
- **Constants**: UPPER_SNAKE_CASE for exported constants (e.g., `API_ENDPOINTS`, `USER_LOGIN`)
- **Functions**: camelCase (e.g., `validateEndpoint`)
- **Object Keys**: UPPER_SNAKE_CASE for constant objects (e.g., `USER_LOGIN`, `CHECK_ELIGIBILITY`)

### Documentation Standards
- **XML Documentation**: Use triple-slash comments (`///`) with `<inheritdoc />` for overridden methods
- **Inline Comments**: Minimal comments; prefer self-documenting code
- **Logging Messages**: Use structured logging with message templates from constants
- **Constants for Messages**: Store all user-facing messages, error messages, and log messages in constant classes

## Architectural Patterns

### Backend Patterns (C#/.NET)

#### Dependency Injection
- **Constructor Injection**: All dependencies injected through constructor
- **Interface-Based**: Always depend on interfaces, not concrete implementations
- **Common Dependencies**: `IUnitOfWork`, `IMapper`, `ILogger<T>`, `IMemoryCache`
```csharp
public EmiCalculatorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmiCalculatorService> logger)
{
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _logger = logger;
}
```

#### Repository and Unit of Work Pattern
- **Repository Access**: Access repositories through `IUnitOfWork` interface
- **Generic Repositories**: Use `ILMPRepository<TEntity, TKey>` for data access
- **Unit of Work Methods**: `BeginTransactionAsync()`, `SaveChangesAsync()`, `CommitAsync()`, `RollbackAsync()`
```csharp
using (var transaction = await _unitOfWork.BeginTransactionAsync())
{
    try
    {
        // Perform operations
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

#### Service Layer Pattern
- **Service Classes**: Implement business logic in service classes
- **Interface Contracts**: Define service contracts in `Interfaces` folder
- **Async Operations**: All I/O operations are asynchronous
- **Return Types**: Use DTOs for service method return types, not entities

#### AutoMapper Pattern
- **Entity to DTO Mapping**: Use AutoMapper for object-to-object mapping
- **Mapping Profiles**: Define mappings in separate profile classes (e.g., `CustomerProfile`, `LoanProductProfile`)
- **Manual Mapping**: Use `_mapper.Map<TDestination>(source)` for conversions
```csharp
var emiPlan = _mapper.Map<EMIPlan>(createDto);
return _mapper.Map<EMIPlanDTO>(emiPlan);
```

#### Logging Pattern
- **Structured Logging**: Use ILogger with message templates
- **Log Levels**: Information for normal flow, Warning for issues, Error for exceptions
- **Constant Messages**: Reference messages from `ApplicationConstants.Messages` and `ApplicationConstants.ErrorMessages`
```csharp
_logger.LogInformation(ApplicationConstants.Messages.ProcessingEMICalculation, amount, term);
_logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlanCreationFailed);
```

#### Exception Handling Pattern
- **Try-Catch Blocks**: Wrap operations in try-catch with specific error messages
- **Transaction Rollback**: Always rollback transactions on exceptions
- **Rethrow with Context**: Throw new exceptions with meaningful messages from constants
- **Conditional Rethrow**: Use `when (!(ex is ArgumentException))` to avoid catching specific exceptions
```csharp
catch (Exception ex) when (!(ex is ArgumentException))
{
    _logger.LogError(ex, "Operation failed");
    throw new Exception(ApplicationConstants.ErrorMessages.OperationFailed);
}
```

### Frontend Patterns (TypeScript/React)

#### Endpoint Validation
- **Validation Function**: Use `validateEndpoint` to ensure endpoint format correctness
- **Endpoint Format**: All endpoints must start with '/' and be strings
- **Constant Export**: Export endpoints as `const` objects with `as const` assertion
```typescript
const validateEndpoint = (endpoint: string): string => {
  if (!endpoint || typeof endpoint !== 'string' || !endpoint.startsWith('/')) {
    throw new Error('Invalid API endpoint format');
  }
  return endpoint;
};
```

#### API Endpoint Organization
- **Grouped by Controller**: Organize endpoints by backend controller (Token, Customer, Eligibility, etc.)
- **Descriptive Names**: Use clear, action-oriented names (e.g., `USER_LOGIN`, `CHECK_ELIGIBILITY`)
- **Consistent Naming**: Match backend route structure in endpoint names

## Common Implementation Patterns

### Async/Await Pattern
- **All I/O Operations**: Database, HTTP, file operations are async
- **Task Return Types**: Use `Task<T>` for async methods with return values, `Task` for void
- **Await Keyword**: Always await async operations
```csharp
public async Task<EMIPlanDTO> CreateEmiPlanAsync(EMIPlanCreateDTO createDto)
{
    var created = await _unitOfWork.EMIPlans.AddAsync(emiPlan);
    await _unitOfWork.SaveChangesAsync();
    return MapToDto(created);
}
```

### Caching Pattern
- **Memory Cache**: Use `IMemoryCache` for temporary data storage
- **Cache Keys**: Use descriptive string keys with entity identifiers (e.g., `"credit_score_{customerId}"`)
- **Cache Expiration**: Set appropriate expiration times (e.g., 24 hours for credit scores)
```csharp
var cacheKey = $"credit_score_{customerId}";
if (_cache.TryGetValue(cacheKey, out CreditScoreDto? cachedScore))
    return cachedScore!;
_cache.Set(cacheKey, creditScore, TimeSpan.FromHours(24));
```

### LINQ Query Pattern
- **Method Syntax**: Prefer method syntax over query syntax
- **Filtering**: Use `Where()`, `FirstOrDefault()`, `Any()` for filtering
- **Aggregation**: Use `Sum()`, `Count()`, `Max()`, `Min()` for calculations
- **Projection**: Use `Select()` for transformations
```csharp
var paidCount = schedule.Count(s => s.PaymentStatus == "Paid");
var totalPaid = payments.Where(p => p.Status == PaymentStatus.Success).Sum(p => p.Amount);
```

### Entity Framework Core Patterns
- **Migrations**: Use code-first migrations with descriptive names (e.g., `InitialCreate`)
- **Fluent API**: Configure relationships and constraints in migration Up/Down methods
- **Data Annotations**: Use attributes for column types, max lengths, and constraints
- **Seed Data**: Insert initial data using `InsertData` in migrations
```csharp
migrationBuilder.CreateTable(
    name: "LoanAccounts",
    columns: table => new
    {
        LoanAccountId = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        TotalLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
    });
```

### Constants Organization Pattern
- **Nested Static Classes**: Organize constants in nested static classes by category
- **Categories**: Roles, Claims, Messages, ErrorMessages, Routes, ContentTypes, etc.
- **String Interpolation**: Use placeholders `{0}`, `{1}` for parameterized messages
```csharp
public static class ApplicationConstants
{
    public static class Messages
    {
        public const string ProcessingEMICalculation = "Processing EMI calculation for amount: {0}, term: {1} months";
    }
    public static class ErrorMessages
    {
        public const string EMIPlanNotFound = "EMI Plan not found";
    }
}
```

### Calculation Pattern
- **Financial Calculations**: Use decimal type for all monetary values
- **Rounding**: Use `Math.Round(value, 2)` for currency amounts
- **Math Operations**: Use `Math.Pow()`, `Math.Max()`, `Math.Min()` for complex calculations
- **Type Casting**: Cast to double for Math.Pow, then back to decimal
```csharp
var monthlyRate = interestRate / 12 / 100;
var emi = (principalAmount * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), termMonths)) /
          ((decimal)Math.Pow((double)(1 + monthlyRate), termMonths) - 1);
return Math.Round(emi, 2);
```

### Notification Pattern
- **Async Notification Creation**: Create notifications asynchronously after operations
- **User Lookup**: Retrieve user information before creating notification
- **Message Format**: Use descriptive messages with currency symbols and details
```csharp
private async Task CreateNotificationAsync(int customerId, string message)
{
    var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
    if (customer != null)
    {
        var notification = new Notification
        {
            UserId = customer.UserId,
            NotificationMessage = message
        };
        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

## Best Practices

### Error Handling
- **Specific Exceptions**: Throw `ArgumentException` for validation errors
- **Null Checks**: Check for null entities and throw appropriate exceptions
- **Logging Before Throwing**: Log errors before throwing exceptions
- **Meaningful Messages**: Use constants for error messages

### Performance Optimization
- **Caching**: Cache frequently accessed, rarely changing data
- **Async Operations**: Use async/await for all I/O operations
- **Lazy Loading**: Avoid N+1 queries with proper includes
- **Batch Operations**: Use transactions for multiple related operations

### Security Practices
- **Input Validation**: Validate all inputs at service layer
- **SQL Injection Prevention**: Use parameterized queries (EF Core handles this)
- **Authentication**: Use JWT tokens for API authentication
- **Authorization**: Check user permissions before operations

### Testing Considerations
- **Testable Code**: Use dependency injection for easy mocking
- **Interface Segregation**: Keep interfaces focused and small
- **Pure Functions**: Prefer pure functions for calculations
- **Separation of Concerns**: Keep business logic separate from data access

### Code Reusability
- **Helper Methods**: Extract common logic into private helper methods
- **Extension Methods**: Use extension methods for cross-cutting concerns
- **Generic Methods**: Use generics for type-safe reusable code
- **Constants**: Centralize all magic strings and numbers

## Technology-Specific Patterns

### Entity Framework Core
- **DbContext Usage**: Access through Unit of Work pattern
- **Transactions**: Use explicit transactions for multi-step operations
- **Eager Loading**: Use `Include()` for related entities when needed
- **Tracking**: Be aware of change tracking behavior

### AutoMapper
- **Profile Classes**: One profile per domain area
- **Reverse Mapping**: Use `ReverseMap()` when bidirectional mapping needed
- **Custom Mappings**: Use `ForMember()` for complex mappings
- **Validation**: Validate mappings at startup

### Serilog
- **Structured Logging**: Use message templates with parameters
- **Log Levels**: Information, Warning, Error, Debug
- **Contextual Information**: Include relevant IDs and parameters
- **File Sinks**: Log to files with date-based naming

### TypeScript
- **Type Safety**: Use strict type checking
- **Const Assertions**: Use `as const` for immutable objects
- **Validation**: Validate runtime data at boundaries
- **Error Handling**: Throw descriptive errors for invalid states
