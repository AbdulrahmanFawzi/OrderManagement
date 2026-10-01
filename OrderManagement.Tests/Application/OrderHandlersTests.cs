using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Orders.Commands.AddOrderItem;
using OrderManagement.Application.Orders.Commands.ConfirmOrder;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Domain.Entities;
using OrderManagement.Tests.Fakes;

namespace OrderManagement.Tests.Application;

// Application use-case tests: verify coordination logic
// (active customer? enough stock? price snapshot?) using fakes.
public class OrderHandlersTests
{
    [Fact]
    public async Task CreateOrder_requires_active_customer()
    {
        var customer = new Customer("lina", "lina@mail.com");
        customer.Deactivate();

        var customers = new FakeCustomerRepository([customer]);
        var orders = new FakeOrderRepository([]);
        var handler = new CreateOrderHandler(orders, customers);

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => handler.Handle(new CreateOrderCommand(customer.Id)));
    }

    [Fact]
    public async Task AddItem_snapshots_price_and_reserves_stock()
    {
        var customer = new Customer("omar", "omar@mail.com");
        var product = new Product("Keyboard", 100m, 5);
        var order = new Order(customer.Id);

        var customers = new FakeCustomerRepository([customer]);
        var products = new FakeProductRepository([product]);
        var orderStore = new List<Order> { order };
        var orders = new FakeOrderRepository(orderStore);

        var add = new AddOrderItemHandler(orders, products);
        var dto = await add.Handle(new AddOrderItemCommand(order.Id, product.Id, 2));

        Assert.Single(dto.Items);
        Assert.Equal(100m, dto.Items[0].UnitPrice);
        Assert.Equal(3, product.Stock); // 5 - 2 reserved

        // Later price change must NOT affect the existing item.
        product.ChangePrice(200m);
        var get = new GetOrderByIdHandler(orders);
        var reloaded = await get.Handle(new GetOrderByIdQuery(order.Id));
        Assert.Equal(100m, reloaded.Items[0].UnitPrice);
    }

    [Fact]
    public async Task Cannot_add_more_than_available_stock()
    {
        var product = new Product("Mouse", 20m, 1);
        var order = new Order(Guid.NewGuid());

        var orders = new FakeOrderRepository([order]);
        var products = new FakeProductRepository([product]);
        var add = new AddOrderItemHandler(orders, products);

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => add.Handle(new AddOrderItemCommand(order.Id, product.Id, 5)));
    }

    [Fact]
    public async Task Confirm_requires_items()
    {
        var order = new Order(Guid.NewGuid());
        var orders = new FakeOrderRepository([order]);
        var confirm = new ConfirmOrderHandler(orders);

        // Domain invariant surfaces through the handler.
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => confirm.Handle(new ConfirmOrderCommand(order.Id)));
    }
}
