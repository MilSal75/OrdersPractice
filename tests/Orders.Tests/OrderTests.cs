using Orders.Models;

namespace Orders.Tests;

public class OrderTests
{
    // Готовый пример теста. Остальные тесты вы пишете по аналогии.
    [Fact]
    public void Total_TwoItems_ReturnsSum()
    {
        // Arrange
        var order = new Order();
        order.Add("Книга", 300m, 2);
        order.Add("Ручка", 50m, 1);

        // Act
        var total = order.Total();

        // Assert
        Assert.Equal(650m, total);
    }


    //1. Проверка на сумму пустого заказа, должна быть равна 0
    [Fact]
    public void Total_EmptyOrder_ReturnsZero()
    {
        // Arrange
        var order = new Order();

        // Act
        var total = order.Total();

        // Assert
        Assert.Equal(0m, total);
    }

    //2. Проверка на верное добавление товара
    [Fact]
    public void Add_ValidItem_AddsToItems()
    {
        // Arrange
        var order = new Order();

        // Act
        order.Add("Книга", 300m, 2);

        // Assert
        var item = Assert.Single(order.Items);

        Assert.Equal("Книга", item.Name);
        Assert.Equal(300m, item.Price);
        Assert.Equal(2, item.Quantity);
    }

    //3. Проверка на добавление товара с нулевой или отрицательной ценой

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_NonPositivePrice_Throws(decimal price)
    {
        // Arrange
        var order = new Order();

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => order.Add("Книга", price, 2)
        );
    }

    //4. Проверка на недопустимое количество товара


    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    public void Add_InvalidQuantity_Throws(int quantity)
    {
        // Arrange
        var order = new Order();

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => order.Add("Книга", 300m, quantity)
        );
    }

    //5. Проверка на пустое название товара


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_InvalidName_Throws(string? name)
    {
        // Arrange
        var order = new Order();

        // Act + Assert
        Assert.Throws<ArgumentException>(
            () => order.Add(name!, 300m, 2)
        );
    }

}
