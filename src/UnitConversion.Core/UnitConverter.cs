using System.Globalization;
using UnitConversion.Core.Models;

namespace UnitConversion.Core;

public interface IUnitConverter
{
    ConversionResult Convert(double value, string fromUnit, string toUnit);
}

/// <summary>
/// Converts in two steps: source unit → base unit → target unit.
/// This way each unit needs only one definition instead of one per pair of units.
/// </summary>
public class UnitConverter : IUnitConverter
{
    public ConversionResult Convert(double value, string fromUnit, string toUnit)
    {
        if (!double.IsFinite(value))
            throw new ConversionException("Value must be a finite number.");

        var from = UnitCatalog.Find(fromUnit) ?? throw new ConversionException($"Unknown unit '{fromUnit}'.");
        var to = UnitCatalog.Find(toUnit) ?? throw new ConversionException($"Unknown unit '{toUnit}'.");

        if (from.Category != to.Category)
            throw new ConversionException($"Cannot convert {from.Category} ({from.Id}) to {to.Category} ({to.Id}).");

        var baseValue = from.ToBase(value);

        if (from.Category == "temperature" && baseValue < 0)
            throw new ConversionException("Temperature cannot be below absolute zero.");

        var result = Round(to.FromBase(baseValue));

        if (!double.IsFinite(result))
            throw new ConversionException("Result is too large.");

        return new ConversionResult(from.Category, value, from, result, to);
    }

    // Removes floating-point noise, e.g. 211.99999999999997 -> 212, by keeping 15 significant digits.
    private static double Round(double value) =>
        double.Parse(value.ToString("G15", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
}
