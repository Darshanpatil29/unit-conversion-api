using UnitConversion.Core;

namespace UnitConversion.Tests;

public class UnitConverterTests
{
    private readonly UnitConverter _converter = new();

    [Theory]
    [InlineData(1, "m", "ft", 3.28083989501312)]
    [InlineData(1, "mile", "km", 1.609344)]
    [InlineData(12, "in", "ft", 1)]
    [InlineData(5, "kg", "lb", 11.0231131092439)]
    [InlineData(16, "oz", "lb", 1)]
    [InlineData(1, "gal", "L", 3.785411784)]
    [InlineData(0, "C", "F", 32)]
    [InlineData(100, "celsius", "fahrenheit", 212)]
    [InlineData(-40, "C", "F", -40)]
    [InlineData(0, "K", "C", -273.15)]
    public void Converts_correctly(double value, string from, string to, double expected)
    {
        Assert.Equal(expected, _converter.Convert(value, from, to).Result, 9);
    }

    [Fact]
    public void Unit_lookup_ignores_case_and_accepts_aliases()
    {
        Assert.Equal(_converter.Convert(1, "m", "ft").Result, _converter.Convert(1, "METERS", "Feet").Result);
    }

    [Fact]
    public void Every_unit_converts_to_every_other_unit_and_back()
    {
        foreach (var from in UnitCatalog.Units)
        {
            foreach (var to in UnitCatalog.Units.Where(u => u.Category == from.Category))
            {
                var forward = _converter.Convert(123.456, from.Id, to.Id).Result;
                var back = _converter.Convert(forward, to.Id, from.Id).Result;
                Assert.Equal(123.456, back, 9);
            }
        }
    }

    [Theory]
    [InlineData(1, "furlong", "m")]          // unknown unit
    [InlineData(1, "m", "kg")]               // different categories
    [InlineData(-300, "C", "K")]             // below absolute zero
    [InlineData(double.NaN, "m", "ft")]      // not a number
    public void Rejects_invalid_conversions(double value, string from, string to)
    {
        Assert.Throws<ConversionException>(() => _converter.Convert(value, from, to));
    }
}
