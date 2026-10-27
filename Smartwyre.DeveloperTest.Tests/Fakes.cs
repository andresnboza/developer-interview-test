using System.Collections.Generic;
using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Tests;

// Hand-written fakes: return whatever the test sets up and record what gets stored
public class FakeRebateDataStore : IRebateDataStore
{
    // Rebate to return when GetRebate is called
    public Rebate Rebate { get; set; }

    // List of rebate calculations stored by the service
    public List<(Rebate Rebate, decimal Amount)> StoredCalculations { get; } = new();

    // Return the Rebate property when GetRebate is called
    public Rebate GetRebate(string rebateIdentifier) => Rebate;

    // Store the rebate and amount in the StoredCalculations list
    public void StoreCalculationResult(Rebate rebate, decimal rebateAmount) =>
        StoredCalculations.Add((rebate, rebateAmount));
}

public class FakeProductDataStore : IProductDataStore
{
    // Product to return when GetProduct is called
    public Product Product { get; set; }

    // Return the Product property when GetProduct is called
    public Product GetProduct(string productIdentifier) => Product;
}

public class FakeIncentiveCalculator : IIncentiveCalculator
{
    // Incentive type and product flag this fake claims to handle
    public IncentiveType Incentive { get; set; } = IncentiveType.FixedCashAmount;
    public SupportedIncentiveType SupportedIncentive { get; set; } = SupportedIncentiveType.FixedCashAmount;

    // Values returned by CanCalculate and Calculate
    public bool CanCalculateResult { get; set; } = true;
    public decimal Amount { get; set; } = 42m;

    // Whether the service asked this calculator for an amount
    public bool CalculateCalled { get; private set; }

    public bool CanCalculate(Rebate rebate, Product product, CalculateRebateRequest request) => CanCalculateResult;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request)
    {
        CalculateCalled = true;
        return Amount;
    }
}
