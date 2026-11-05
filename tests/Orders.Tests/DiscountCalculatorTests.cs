// 7. Тест для проверки расчета скидки
using Orders.Services;
using Xunit;

namespace Orders.Tests;

public class DiscountCalculatorTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(999.99, 0)]
    [InlineData(1000, 100)]
    [InlineData(2000, 200)]
    public void Calculate_ReturnsExpected(decimal total, decimal expected)
    {
        // Arrange
        var calculator = new DiscountCalculator();

        //При выполнении unit-теста Calculate_ReturnsExpected обнаружена ошибка в классе DiscountCalculator.
        //Для суммы заказа 1000 рублей метод возвращал скидку 0 вместо 100 рублей.
        //Причиной было использование оператора > вместо >=. После исправления условия расчёта скидки
        //тест успешно проходит.

        // Act
        var actual = calculator.Calculate(total);

        // Assert
        Assert.Equal(expected, actual);
    }

    // 9. Проверка скидки от отрицательной суммы заказа, должна выбрасываться ошибка
    [Fact]
    public void Calculate_NegativeTotal_Throws()
    {
        // Arrange
        var calculator = new DiscountCalculator();

        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.Calculate(-100m)
        );
    }

}
