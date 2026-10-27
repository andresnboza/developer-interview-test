namespace Smartwyre.DeveloperTest.Types;

public class CalculateRebateResult
{
    public bool Success { get; set; }

    public decimal RebateAmount { get; set; }

    // Explains why the calculation failed; null on success
    public string Message { get; set; }
}
