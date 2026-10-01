namespace OrderManagement.Application.Customers.Dtos;

// Read model returned by Customer queries/commands.
// Handlers map the Domain Customer entity to this DTO so callers
// (API, tests) never depend on persistence details.
public record CustomerDto(
    Guid Id,
    string Username,
    string Email,
    bool IsActive,
    DateTime CreatedAt);
