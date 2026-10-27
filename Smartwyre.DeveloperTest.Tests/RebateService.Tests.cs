using System;
using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private const SupportedIncentiveType AllIncentives =
        SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom;

    private readonly FakeRebateDataStore _rebateDataStore = new();
    private readonly FakeProductDataStore _productDataStore = new();
    private readonly RebateService _sut;

    // The real calculators are used here so these tests check end-to-end results;
    // see the "service with fake calculator" tests below for the service on its own
    private static IIncentiveCalculator[] AllCalculators() =>
    [
        new FixedCashAmountCalculator(),
        new FixedRateRebateCalculator(),
        new AmountPerUomCalculator(),
    ];

    public RebateServiceTests()
    {
        _sut = new RebateService(_rebateDataStore, _productDataStore, AllCalculators());
    }

    private CalculateRebateResult Calculate(decimal volume = 10m) =>
        _sut.Calculate(new CalculateRebateRequest { RebateIdentifier = "R1", ProductIdentifier = "P1", Volume = volume });

    private static Product ProductSupporting(SupportedIncentiveType incentives, decimal price = 20m) =>
        new() { Identifier = "P1", Price = price, SupportedIncentives = incentives };

    [Fact]
    public void FixedCashAmount_ReturnsRebateAmount_AndStoresIt()
    {
        _rebateDataStore.Rebate = new Rebate { Identifier = "R1", Incentive = IncentiveType.FixedCashAmount, Amount = 500m };
        _productDataStore.Product = ProductSupporting(SupportedIncentiveType.FixedCashAmount);

        var result = Calculate();

        Assert.True(result.Success);
        Assert.Equal(500m, result.RebateAmount);
        var stored = Assert.Single(_rebateDataStore.StoredCalculations);
        Assert.Same(_rebateDataStore.Rebate, stored.Rebate);
        Assert.Equal(500m, stored.Amount);
    }

    [Fact]
    public void FixedRateRebate_ReturnsPriceTimesPercentageTimesVolume()
    {
        _rebateDataStore.Rebate = new Rebate { Incentive = IncentiveType.FixedRateRebate, Percentage = 0.05m };
        _productDataStore.Product = ProductSupporting(SupportedIncentiveType.FixedRateRebate, price: 20m);

        var result = Calculate(volume: 100m);

        Assert.True(result.Success);
        Assert.Equal(100m, result.RebateAmount);
        Assert.Equal(100m, Assert.Single(_rebateDataStore.StoredCalculations).Amount);
    }

    [Fact]
    public void AmountPerUom_ReturnsAmountTimesVolume()
    {
        _rebateDataStore.Rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 2m };
        _productDataStore.Product = ProductSupporting(SupportedIncentiveType.AmountPerUom);

        var result = Calculate(volume: 100m);

        Assert.True(result.Success);
        Assert.Equal(200m, result.RebateAmount);
        Assert.Equal(200m, Assert.Single(_rebateDataStore.StoredCalculations).Amount);
    }

    [Fact]
    public void RebateNotFound_Fails_WithoutStoring()
    {
        _rebateDataStore.Rebate = null;
        _productDataStore.Product = ProductSupporting(AllIncentives);

        var result = Calculate();

        Assert.False(result.Success);
        Assert.Empty(_rebateDataStore.StoredCalculations);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedRateRebate)]
    [InlineData(IncentiveType.AmountPerUom)]
    public void ProductNotFound_Fails_WithoutStoring(IncentiveType incentive)
    {
        _rebateDataStore.Rebate = new Rebate { Incentive = incentive, Amount = 5m, Percentage = 0.1m };
        _productDataStore.Product = null;

        var result = Calculate();

        Assert.False(result.Success);
        Assert.Empty(_rebateDataStore.StoredCalculations);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate)]
    public void IncentiveNotSupportedByProduct_Fails(IncentiveType incentive, SupportedIncentiveType supported)
    {
        _rebateDataStore.Rebate = new Rebate { Incentive = incentive, Amount = 5m, Percentage = 0.1m };
        _productDataStore.Product = ProductSupporting(supported);

        var result = Calculate();

        Assert.False(result.Success);
        Assert.Empty(_rebateDataStore.StoredCalculations);
    }

    [Theory]
    // incentive, rebate amount, rebate percentage, product price, volume
    [InlineData(IncentiveType.FixedCashAmount, 0, 0, 20, 10)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 0, 20, 10)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 0.05, 0, 10)]
    [InlineData(IncentiveType.FixedRateRebate, 0, 0.05, 20, 0)]
    [InlineData(IncentiveType.AmountPerUom, 0, 0, 20, 10)]
    [InlineData(IncentiveType.AmountPerUom, 2, 0, 20, 0)]
    public void ZeroInputs_Fail(IncentiveType incentive, decimal amount, decimal percentage, decimal price, decimal volume)
    {
        _rebateDataStore.Rebate = new Rebate { Incentive = incentive, Amount = amount, Percentage = percentage };
        _productDataStore.Product = ProductSupporting(AllIncentives, price);

        var result = Calculate(volume);

        Assert.False(result.Success);
        Assert.Empty(_rebateDataStore.StoredCalculations);
    }

    [Fact]
    public void Constructor_ThrowsOnNullDependencies()
    {
        Assert.Throws<ArgumentNullException>(() => new RebateService(null, _productDataStore, AllCalculators()));
        Assert.Throws<ArgumentNullException>(() => new RebateService(_rebateDataStore, null, AllCalculators()));
        Assert.Throws<ArgumentNullException>(() => new RebateService(_rebateDataStore, _productDataStore, null));
    }

    [Fact]
    public void Constructor_ThrowsWhenTwoCalculatorsHandleTheSameIncentive()
    {
        var calculators = new[] { new FakeIncentiveCalculator(), new FakeIncentiveCalculator() };

        Assert.Throws<ArgumentException>(() => new RebateService(_rebateDataStore, _productDataStore, calculators));
    }

    // --- Service with fake calculator: checks the service's own logic, independent of any real incentive rules

    [Fact]
    public void UsesTheCalculatorRegisteredForTheRebatesIncentive()
    {
        var calculator = new FakeIncentiveCalculator { Amount = 123m };
        var sut = new RebateService(_rebateDataStore, _productDataStore, [calculator]);
        _rebateDataStore.Rebate = new Rebate { Incentive = calculator.Incentive };
        _productDataStore.Product = ProductSupporting(calculator.SupportedIncentive);

        var result = sut.Calculate(new CalculateRebateRequest());

        Assert.True(result.Success);
        Assert.Null(result.Message);
        Assert.Equal(123m, result.RebateAmount);
        Assert.Equal(123m, Assert.Single(_rebateDataStore.StoredCalculations).Amount);
    }

    [Fact]
    public void NoCalculatorForIncentive_Fails_WithMessage()
    {
        var sut = new RebateService(_rebateDataStore, _productDataStore, []);
        _rebateDataStore.Rebate = new Rebate { Incentive = IncentiveType.AmountPerUom, Amount = 2m };
        _productDataStore.Product = ProductSupporting(AllIncentives);

        var result = sut.Calculate(new CalculateRebateRequest { Volume = 10m });

        Assert.False(result.Success);
        Assert.Contains("No calculator", result.Message);
        Assert.Empty(_rebateDataStore.StoredCalculations);
    }

    [Fact]
    public void CalculatorRejectsInputs_Fails_WithoutCalculatingOrStoring()
    {
        var calculator = new FakeIncentiveCalculator { CanCalculateResult = false };
        var sut = new RebateService(_rebateDataStore, _productDataStore, [calculator]);
        _rebateDataStore.Rebate = new Rebate { Incentive = calculator.Incentive };
        _productDataStore.Product = ProductSupporting(calculator.SupportedIncentive);

        var result = sut.Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
        Assert.False(calculator.CalculateCalled);
        Assert.Empty(_rebateDataStore.StoredCalculations);
    }

    [Fact]
    public void ProductNotSupportingCalculatorsIncentive_Fails_WithoutCalculating()
    {
        var calculator = new FakeIncentiveCalculator();
        var sut = new RebateService(_rebateDataStore, _productDataStore, [calculator]);
        _rebateDataStore.Rebate = new Rebate { Incentive = calculator.Incentive };
        _productDataStore.Product = ProductSupporting(SupportedIncentiveType.AmountPerUom);

        var result = sut.Calculate(new CalculateRebateRequest());

        Assert.False(result.Success);
        Assert.Contains("does not support", result.Message);
        Assert.False(calculator.CalculateCalled);
    }
}
