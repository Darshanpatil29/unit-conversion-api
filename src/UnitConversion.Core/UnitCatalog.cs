using UnitConversion.Core.Models;

namespace UnitConversion.Core;

/// <summary>
/// All supported units, hardcoded. Each category has one base unit (factor 1) and every other
/// unit is defined against it. To add a unit, add one line to the list below.
/// </summary>
public static class UnitCatalog
{
    public static IReadOnlyList<Unit> Units { get; } =
    [
        // Length (base: meter)
        new("meter", "Meter", "m", "length", 1, 0, "meters", "metre"),
        new("kilometer", "Kilometer", "km", "length", 1000, 0, "kilometers"),
        new("centimeter", "Centimeter", "cm", "length", 0.01, 0, "centimeters"),
        new("millimeter", "Millimeter", "mm", "length", 0.001, 0, "millimeters"),
        new("inch", "Inch", "in", "length", 0.0254, 0, "inches"),
        new("foot", "Foot", "ft", "length", 0.3048, 0, "feet"),
        new("yard", "Yard", "yd", "length", 0.9144, 0, "yards"),
        new("mile", "Mile", "mi", "length", 1609.344, 0, "miles"),

        // Weight / mass (base: kilogram)
        new("kilogram", "Kilogram", "kg", "mass", 1, 0, "kilograms"),
        new("gram", "Gram", "g", "mass", 0.001, 0, "grams"),
        new("milligram", "Milligram", "mg", "mass", 0.000001, 0, "milligrams"),
        new("tonne", "Tonne", "t", "mass", 1000, 0, "tonnes"),
        new("pound", "Pound", "lb", "mass", 0.45359237, 0, "pounds", "lbs"),
        new("ounce", "Ounce", "oz", "mass", 0.028349523125, 0, "ounces"),

        // Temperature (base: kelvin). baseValue = (value + Offset) * Factor
        new("kelvin", "Kelvin", "K", "temperature", 1),
        new("celsius", "Celsius", "°C", "temperature", 1, 273.15, "C"),
        new("fahrenheit", "Fahrenheit", "°F", "temperature", 5.0 / 9.0, 459.67, "F"),

        // Volume (base: liter)
        new("liter", "Liter", "L", "volume", 1, 0, "liters", "litre"),
        new("milliliter", "Milliliter", "mL", "volume", 0.001, 0, "milliliters"),
        new("gallon", "Gallon (US)", "gal", "volume", 3.785411784, 0, "gallons"),
    ];

    // Lookup by id, symbol or alias, ignoring case. Built once; throws at startup if two units share a key.
    private static readonly Dictionary<string, Unit> UnitsByKey = BuildLookup();

    public static IEnumerable<string> Categories => Units.Select(u => u.Category).Distinct();

    public static Unit? Find(string key) => UnitsByKey.GetValueOrDefault(key.Trim());

    private static Dictionary<string, Unit> BuildLookup()
    {
        var lookup = new Dictionary<string, Unit>(StringComparer.OrdinalIgnoreCase);
        foreach (var unit in Units)
        {
            foreach (var key in new[] { unit.Id, unit.Symbol }.Concat(unit.Aliases).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!lookup.TryAdd(key, unit))
                {
                    throw new InvalidOperationException($"Duplicate unit key '{key}' ({unit.Id} and {lookup[key].Id}).");
                }
            }
        }
        return lookup;
    }
}
