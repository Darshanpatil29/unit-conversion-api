namespace UnitConversion.Core;

/// <summary>Thrown when a conversion request is invalid (unknown unit, incompatible units, impossible value).</summary>
public class ConversionException(string message) : Exception(message);
