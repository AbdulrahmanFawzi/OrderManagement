using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Customers.Commands.ActivateCustomer;
using OrderManagement.Application.Customers.Commands.CreateCustomer;
using OrderManagement.Application.Customers.Commands.DeactivateCustomer;
using OrderManagement.Application.Customers.Queries.GetCustomerById;
using OrderManagement.Application.Customers.Queries.GetCustomers;
using OrderManagement.Application.Orders.Commands.AddOrderItem;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Application.Orders.Commands.ConfirmOrder;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Orders.Commands.DeliverOrder;
using OrderManagement.Application.Orders.Commands.RemoveOrderItem;
using OrderManagement.Application.Orders.Commands.ShipOrder;
using OrderManagement.Application.Orders.Commands.StartProcessingOrder;
using OrderManagement.Application.Orders.Queries.GetCustomerOrders;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Application.Products.Commands.ActivateProduct;
using OrderManagement.Application.Products.Commands.AdjustStock;
using OrderManagement.Application.Products.Commands.ChangeProductPrice;
using OrderManagement.Application.Products.Commands.CreateProduct;
using OrderManagement.Application.Products.Commands.DeactivateProduct;
using OrderManagement.Application.Products.Queries.GetProductById;
using OrderManagement.Application.Products.Queries.GetProducts;

namespace OrderManagement.Application;

// Central place to register use cases.
// No MediatR here on purpose: controllers inject these handlers
// directly, so the CQRS flow stays visible for learning.
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Customers
        services.AddScoped<CreateCustomerHandler>();
        services.AddScoped<ActivateCustomerHandler>();
        services.AddScoped<DeactivateCustomerHandler>();
        services.AddScoped<GetCustomerByIdHandler>();
        services.AddScoped<GetCustomersHandler>();

        // Products
        services.AddScoped<CreateProductHandler>();
        services.AddScoped<ChangeProductPriceHandler>();
        services.AddScoped<IncreaseStockHandler>();
        services.AddScoped<DecreaseStockHandler>();
        services.AddScoped<ActivateProductHandler>();
        services.AddScoped<DeactivateProductHandler>();
        services.AddScoped<GetProductByIdHandler>();
        services.AddScoped<GetProductsHandler>();

        // Orders
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<AddOrderItemHandler>();
        services.AddScoped<RemoveOrderItemHandler>();
        services.AddScoped<ConfirmOrderHandler>();
        services.AddScoped<CancelOrderHandler>();
        services.AddScoped<StartProcessingOrderHandler>();
        services.AddScoped<ShipOrderHandler>();
        services.AddScoped<DeliverOrderHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        services.AddScoped<GetCustomerOrdersHandler>();

        return services;
    }
}
