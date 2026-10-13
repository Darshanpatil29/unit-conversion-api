namespace UnitConversion.Core.Models;

public record ConversionResult(string Category, double Value, Unit From, double Result, Unit To);
