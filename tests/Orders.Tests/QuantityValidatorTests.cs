// 6. Проверка количества товара на возможность добавления такого товара
using Orders.Validation;
using Xunit;

namespace Orders.Tests;

public class QuantityValidatorTests
{
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void IsValidQty_ChecksBounds(int quantity, bool expected)
    {
        // Act
        bool actual = QuantityValidator.IsValidQty(quantity);

        // Assert
        Assert.Equal(expected, actual);
    }
}
