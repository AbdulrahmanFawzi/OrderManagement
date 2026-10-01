namespace OrderManagement.Application.Common.Exceptions;

// Thrown when a requested aggregate (Customer, Product, Order) does not exist.
// The API middleware translates this to HTTP 404.
public sealed class NotFoundException(string message) : Exception(message);
