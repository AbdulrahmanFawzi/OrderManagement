namespace OrderManagement.Application.Common.Exceptions;

// Thrown when an application-level business rule is violated
// (e.g. inactive customer, duplicate username, insufficient stock).
// Domain invariants throw InvalidOperationException / ArgumentException,
// which the middleware also maps to HTTP 400.
// This type exists so application-layer rule violations are explicit.
public sealed class BusinessRuleException(string message) : Exception(message);
