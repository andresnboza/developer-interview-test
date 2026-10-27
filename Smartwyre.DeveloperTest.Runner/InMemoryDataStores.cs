using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

// Seeded stand-ins for the database so the console app has data to calculate against

public class InMemoryRebateDataStore : IRebateDataStore
{
    // Seeded in-memory data store for rebates
    private readonly Dictionary<string, Rebate> _rebates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CASH"] = new Rebate { Identifier = "CASH", Incentive = IncentiveType.FixedCashAmount, Amount = 500m },
        ["RATE"] = new Rebate { Identifier = "RATE", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.05m },
        ["UOM"] = new Rebate { Identifier = "UOM", Incentive = IncentiveType.AmountPerUom, Amount = 2m },
    };

    // Expose the identifiers of the rebates available in the data store
    public IEnumerable<string> Identifiers => _rebates.Keys;

    // Get a rebate by its identifier, returning null if not found
    public Rebate GetRebate(string rebateIdentifier) =>
        _rebates.GetValueOrDefault(rebateIdentifier ?? string.Empty);

    // Store the result of a rebate calculation (for demonstration purposes, just write to console)
    public void StoreCalculationResult(Rebate rebate, decimal rebateAmount) =>
        Console.WriteLine($"[store] Saved calculation of {rebateAmount} for rebate '{rebate.Identifier}'");
}

public class InMemoryProductDataStore : IProductDataStore
{
    // Seeded in-memory data store for products
    private readonly Dictionary<string, Product> _products = new(StringComparer.OrdinalIgnoreCase)
    {
        ["WIDGET"] = new Product
        {
            Id = 1,
            Identifier = "WIDGET",
            Price = 20m,
            Uom = "case",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
                | SupportedIncentiveType.FixedRateRebate
                | SupportedIncentiveType.AmountPerUom,
        },
        ["GADGET"] = new Product
        {
            Id = 2,
            Identifier = "GADGET",
            Price = 50m,
            Uom = "pallet",
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
        },
    };

    // Exposes the identifiers of the products available in the data store
    public IEnumerable<string> Identifiers => _products.Keys;

    // Get a product by its identifier, returning null if not found
    public Product GetProduct(string productIdentifier) =>
        _products.GetValueOrDefault(productIdentifier ?? string.Empty);
}
