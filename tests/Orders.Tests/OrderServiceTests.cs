using Moq;
using Orders.Abstractions;
using Orders.Models;
using Orders.Services;
using Xunit;

namespace Orders.Tests;

public class OrderServiceTests
{
    readonly Mock<IOrderRepository> _repo = new(); //поддельное хранилище заказов
    readonly Mock<IMessagePublisher> _pub = new(); //поддельная система сообщений
    readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(
            _repo.Object,
            _pub.Object,
            new DiscountCalculator()
        );
    }

    static NewOrder Cmd() =>
        new(new List<OrderItem>
        {
            new("Книга", 300m, 2)
        });

    // Тест 1: заказ сохраняется
    [Fact]
    public async Task CreateAsync_ValidOrder_SavesOrder()
    {
        // Arrange
        _repo.Setup(r => r.SaveAsync(It.IsAny<Order>()))
             .Returns(Task.CompletedTask);

        _pub.Setup(p => p.PublishAsync(It.IsAny<OrderCreated>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.CreateAsync(Cmd());

        // Assert
        _repo.Verify(
            r => r.SaveAsync(It.IsAny<Order>()),
            Times.Once()
        );
    }

    // Тест 2: событие публикуется с правильным Id
    [Fact]
    public async Task CreateAsync_ValidOrder_PublishesEventWithSameId()
    {
        // Arrange
        _repo.Setup(r => r.SaveAsync(It.IsAny<Order>()))
             .Returns(Task.CompletedTask);

        _pub.Setup(p => p.PublishAsync(It.IsAny<OrderCreated>()))
            .Returns(Task.CompletedTask);

        // Act
        var id = await _sut.CreateAsync(Cmd());

        // Assert
        _pub.Verify(
            p => p.PublishAsync(
                It.Is<OrderCreated>(e => e.OrderId == id)
            ),
            Times.Once()
        );
    }

    // Тест 3: если сохранение не удалось,
    // событие не публикуется
    [Fact]
    public async Task CreateAsync_SaveFails_DoesNotPublish()
    {
        // Arrange
        _repo.Setup(r => r.SaveAsync(It.IsAny<Order>()))
             .ThrowsAsync(new TimeoutException());

        // Act + Assert
        await Assert.ThrowsAsync<TimeoutException>(
            () => _sut.CreateAsync(Cmd())
        );

        _pub.Verify(
            p => p.PublishAsync(It.IsAny<OrderCreated>()),
            Times.Never()
        );
    }

    //13. Передаем null вместо команды


    [Fact]
    public async Task CreateAsync_NullCommand_Throws()
    {
        // Act + Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _sut.CreateAsync(null!)
        );
    }

    //14. Передаем команду с пустым списком товаров


    [Fact]
    public async Task CreateAsync_EmptyItems_ThrowsAndDoesNotSave()
    {
        // Arrange
        var cmd = new NewOrder(new List<OrderItem>());

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.CreateAsync(cmd)
        );

        _repo.Verify(
            r => r.SaveAsync(It.IsAny<Order>()),
            Times.Never()
        );
    }

    //15. Заказ сохранился, но публикация события не удалась

    [Fact]
    public async Task CreateAsync_PublishFails_ExceptionPropagates()
    {
        // Arrange
        _repo.Setup(r => r.SaveAsync(It.IsAny<Order>()))
             .Returns(Task.CompletedTask);

        _pub.Setup(p => p.PublishAsync(It.IsAny<OrderCreated>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateAsync(Cmd())
        );
    }

}
