// 8. Пытаемся найти заказ, которого нет

using Orders.Models;
using Orders.Repositories;
using Xunit;

namespace Orders.Tests;

public class InMemoryOrderRepositoryTests
{
    [Fact]
    public async Task FindAsync_UnknownId_ReturnsNull()
    {
        // Arrange
        var repo = new InMemoryOrderRepository();
        var id = Guid.NewGuid();

        // Act
        var result = await repo.FindAsync(id);

        // Assert
        Assert.Null(result);
    }

    // 10. Проверка добавления заказа в репозиторий и его поиска

    [Fact]
    public async Task SaveAsync_ThenFindAsync_ReturnsSameOrder()
    {
        // Arrange
        var repo = new InMemoryOrderRepository();
        var order = new Order();

        // Act
        await repo.SaveAsync(order);
        var result = await repo.FindAsync(order.Id);

        // Assert
        Assert.Same(order, result); //ссылка на одинаковый объект в памяти
    }

}
