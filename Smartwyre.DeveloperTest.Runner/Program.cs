using System;
using System.Globalization;
using Smartwyre.DeveloperTest.Calculators;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static int Main(string[] args)
    {
        var rebateDataStore = new InMemoryRebateDataStore(); // rebates data
        var productDataStore = new InMemoryProductDataStore(); // products data

        // One calculator per incentive type; register new incentive types here
        IIncentiveCalculator[] calculators =
        [
            new FixedCashAmountCalculator(),
            new FixedRateRebateCalculator(),
            new AmountPerUomCalculator(),
        ];

        var rebateService = new RebateService(rebateDataStore, productDataStore, calculators); // service to calculate rebates

        string rebateId, productId, volumeText;  // rebate identifier, product identifier and volume purchased

        if (args.Length == 3)
        {
            (rebateId, productId, volumeText) = (args[0], args[1], args[2]);
        }
        else if (args.Length == 0)
        {
            Console.WriteLine($"Rebates available: {string.Join(", ", rebateDataStore.Identifiers)}");
            Console.WriteLine($"Products available: {string.Join(", ", productDataStore.Identifiers)}");
            rebateId = Prompt("Rebate identifier");
            productId = Prompt("Product identifier");
            volumeText = Prompt("Volume"); // this can be a decimal number, e.g. 1.5
        }
        else
        {
            Console.Error.WriteLine("Usage: <rebateId> <productId> <volume>");
            return 1;
        }

        if (!decimal.TryParse(volumeText, NumberStyles.Number, CultureInfo.InvariantCulture, out var volume))
        {
            Console.Error.WriteLine($"Invalid volume: '{volumeText}'");
            return 1;
        }

        var result = rebateService.Calculate(new CalculateRebateRequest
        {
            RebateIdentifier = rebateId,
            ProductIdentifier = productId,
            Volume = volume,
        });

        Console.WriteLine(result.Success
            ? $"Success: rebate amount is {result.RebateAmount}"
            : $"Rebate could not be calculated: {result.Message}");

        return result.Success ? 0 : 2;
    }

    private static string Prompt(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}
