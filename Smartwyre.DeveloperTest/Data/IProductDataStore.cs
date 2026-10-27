using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

/// <summary>
/// Read access to the products that rebates can be calculated against.
/// </summary>
public interface IProductDataStore
{
    /// <summary>
    /// Looks up a product by its identifier.
    /// </summary>
    /// <param name="productIdentifier">The identifier of the product to retrieve.</param>
    /// <returns>The matching product, or <c>null</c> if none exists.</returns>
    Product GetProduct(string productIdentifier);
}
