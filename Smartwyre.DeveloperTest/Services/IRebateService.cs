using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

/// <summary>
/// Calculates rebate amounts for a product purchase and records successful calculations.
/// </summary>
public interface IRebateService
{
    /// <summary>
    /// Calculates the rebate owed for the requested rebate, product and volume.
    /// A successful calculation is also stored.
    /// </summary>
    /// <param name="request">The rebate identifier, product identifier and volume purchased.</param>
    /// <returns>
    /// A result with <c>Success</c> set to <c>true</c> and the calculated <c>RebateAmount</c>;
    /// or <c>Success</c> set to <c>false</c> when the rebate or product is not found, no calculator is
    /// registered for the rebate's incentive type, the product does not support that incentive type,
    /// or a required value is zero.
    /// </returns>
    CalculateRebateResult Calculate(CalculateRebateRequest request);
}
