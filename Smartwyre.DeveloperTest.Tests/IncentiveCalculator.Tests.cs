using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

// Each calculator is tested on its own: no data stores or service involved

public class FixedCashAmountCalculatorTests
{
    private readonly FixedCashAmountCalculator _sut = new();

    [Fact]
    public void HandlesFixedCashAmount()
    {
        Assert.Equal(IncentiveType.FixedCashAmount, _sut.Incentive);
        Assert.Equal(SupportedIncentiveType.FixedCashAmount, _sut.SupportedIncentive);
    }

    [Fact]
    public void Calculate_ReturnsRebateAmount_RegardlessOfVolume()
    {
        var rebate = new Rebate { Amount = 500m };
        var request = new CalculateRebateRequest { Volume = 3m };

        Assert.True(_sut.CanCalculate(rebate, new Product(), request));
        Assert.Equal(500m, _sut.Calculate(rebate, new Product(), request));
    }

    [Fact]
    public void CanCalculate_IsFalse_WhenAmountIsZero()
    {
        Assert.False(_sut.CanCalculate(new Rebate { Amount = 0m }, new Product(), new CalculateRebateRequest()));
    }
}

public class FixedRateRebateCalculatorTests
{
    private readonly FixedRateRebateCalculator _sut = new();

    [Fact]
    public void HandlesFixedRateRebate()
    {
        Assert.Equal(IncentiveType.FixedRateRebate, _sut.Incentive);
        Assert.Equal(SupportedIncentiveType.FixedRateRebate, _sut.SupportedIncentive);
    }

    [Fact]
    public void Calculate_ReturnsPriceTimesPercentageTimesVolume()
    {
        var rebate = new Rebate { Percentage = 0.05m };
        var product = new Product { Price = 20m };
        var request = new CalculateRebateRequest { Volume = 100m };

        Assert.True(_sut.CanCalculate(rebate, product, request));
        Assert.Equal(100m, _sut.Calculate(rebate, product, request));
    }

    [Theory]
    // percentage, price, volume
    [InlineData(0, 20, 100)]
    [InlineData(0.05, 0, 100)]
    [InlineData(0.05, 20, 0)]
    public void CanCalculate_IsFalse_WhenAnyInputIsZero(decimal percentage, decimal price, decimal volume)
    {
        Assert.False(_sut.CanCalculate(
            new Rebate { Percentage = percentage },
            new Product { Price = price },
            new CalculateRebateRequest { Volume = volume }));
    }
}

public class AmountPerUomCalculatorTests
{
    private readonly AmountPerUomCalculator _sut = new();

    [Fact]
    public void HandlesAmountPerUom()
    {
        Assert.Equal(IncentiveType.AmountPerUom, _sut.Incentive);
        Assert.Equal(SupportedIncentiveType.AmountPerUom, _sut.SupportedIncentive);
    }

    [Fact]
    public void Calculate_ReturnsAmountTimesVolume()
    {
        var rebate = new Rebate { Amount = 2m };
        var request = new CalculateRebateRequest { Volume = 100m };

        Assert.True(_sut.CanCalculate(rebate, new Product(), request));
        Assert.Equal(200m, _sut.Calculate(rebate, new Product(), request));
    }

    [Theory]
    // amount, volume
    [InlineData(0, 100)]
    [InlineData(2, 0)]
    public void CanCalculate_IsFalse_WhenAnyInputIsZero(decimal amount, decimal volume)
    {
        Assert.False(_sut.CanCalculate(
            new Rebate { Amount = amount },
            new Product(),
            new CalculateRebateRequest { Volume = volume }));
    }
}
