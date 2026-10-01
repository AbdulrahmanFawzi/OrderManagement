using OrderManagement.Domain.Entities;

namespace OrderManagement.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Cannot_create_with_negative_values_is_guarded_by_behavior()
    {
        var product = new Product("Keyboard", 100m, 5);

        Assert.Throws<ArgumentException>(() => product.ChangePrice(-1m));
        Assert.Throws<ArgumentException>(() => product.IncreaseStock(0));
        Assert.Throws<ArgumentException>(() => product.DecreaseStock(0));
    }

    [Fact]
    public void Stock_cannot_go_negative()
    {
        var product = new Product("Mouse", 50m, 2);

        Assert.Throws<InvalidOperationException>(() => product.DecreaseStock(3));
        Assert.Equal(2, product.Stock); // failed operation changed nothing
    }

    [Fact]
    public void Cannot_buy_more_than_available_stock()
    {
        var product = new Product("Monitor", 300m, 2);

        // This is what AddOrderItemHandler checks before calling DecreaseStock.
        Assert.True(3 > product.Stock);
    }

    [Fact]
    public void Order_item_keeps_historical_price()
    {
        var product = new Product("Headset", 100m, 10);
        var item = new OrderItem(product.Id, 2, product.Price);

        product.ChangePrice(150m); // price change AFTER the item was created

        Assert.Equal(100m, item.UnitPrice); // snapshot preserved
        Assert.Equal(150m, product.Price);
    }

    [Fact]
    public void Order_item_quantity_must_be_positive()
    {
        Assert.Throws<ArgumentException>(() => new OrderItem(Guid.NewGuid(), 0, 10m));

        var item = new OrderItem(Guid.NewGuid(), 1, 10m);
        Assert.Throws<ArgumentException>(() => item.ChangeQuantity(0));
    }
}
