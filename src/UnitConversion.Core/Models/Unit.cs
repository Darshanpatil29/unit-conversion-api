namespace UnitConversion.Core.Models;

/// <summary>
/// A unit of measurement, defined relative to the base unit of its category:
/// <c>baseValue = (value + Offset) * Factor</c>.
/// Most units only need a factor (offset 0). Temperatures also need an offset.
/// </summary>
public record Unit(
    string Id,
    string Name,
    string Symbol,
    string Category,
    double Factor,
    double Offset = 0,
    params string[] Aliases)
{
    public double ToBase(double value) => (value + Offset) * Factor;

    public double FromBase(double baseValue) => baseValue / Factor - Offset;
}
