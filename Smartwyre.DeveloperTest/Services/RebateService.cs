using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IReadOnlyDictionary<IncentiveType, IIncentiveCalculator> _calculators;

    // Dependencies are injected so they can be replaced with fakes in tests.
    // Adding a new incentive type means adding a calculator; this class does not change.
    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IEnumerable<IIncentiveCalculator> calculators)
    {
        _rebateDataStore = rebateDataStore ?? throw new ArgumentNullException(nameof(rebateDataStore));
        _productDataStore = productDataStore ?? throw new ArgumentNullException(nameof(productDataStore));
        ArgumentNullException.ThrowIfNull(calculators);

        // Just in case, we don't want to allow a null calculator in the collection.
        if (calculators.Any(c => c == null))
        {
            throw new ArgumentException("Calculators collection cannot contain null", nameof(calculators));
        }
        // ToDictionary throws if two calculators claim the same incentive type, surfacing the mistake at startup
        _calculators = calculators.ToDictionary(c => c.Incentive);
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        // Evaluating first the product and the rebate before continuing.
        if (rebate == null || product == null)
        {
            return Failed(rebate == null ? "Rebate not found" : "Product not found");
        }

        if (!_calculators.TryGetValue(rebate.Incentive, out var calculator))
        {
            return Failed($"No calculator registered for incentive type {rebate.Incentive}");
        }

        // Checks shared by every incentive type live here; type-specific checks live in the calculator
        if (!product.SupportedIncentives.HasFlag(calculator.SupportedIncentive))
        {
            return Failed($"Product does not support incentive type {rebate.Incentive}");
        }

        if (!calculator.CanCalculate(rebate, product, request))
        {
            return Failed("Rebate cannot be calculated with the given values");
        }

        var rebateAmount = calculator.Calculate(rebate, product, request);
        _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);

        return new CalculateRebateResult { Success = true, RebateAmount = rebateAmount };
    }

    private static CalculateRebateResult Failed(string message) =>
        new() { Success = false, Message = message };
}
