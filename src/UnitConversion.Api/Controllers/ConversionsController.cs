using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UnitConversion.Core;
using UnitConversion.Core.Models;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api")]
public class ConversionsController(IUnitConverter converter) : ControllerBase
{
    /// <summary>Converts a value from one unit to another.</summary>
    /// <param name="value">The number to convert, e.g. 100.</param>
    /// <param name="from">Source unit (id, symbol or alias), e.g. celsius, °C or C.</param>
    /// <param name="to">Target unit (id, symbol or alias), e.g. fahrenheit, °F or F.</param>
    /// <response code="200">The converted value.</response>
    /// <response code="400">Unknown unit, incompatible units or invalid value.</response>
    [HttpGet("convert")]
    [ProducesResponseType<ConversionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public IActionResult Convert([FromQuery, BindRequired] double value, [FromQuery] string from, [FromQuery] string to)
    {
        try
        {
            return Ok(converter.Convert(value, from, to));
        }
        catch (ConversionException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Conversion failed");
        }
    }

    /// <summary>Lists the supported categories, e.g. length, mass, temperature, volume.</summary>
    [HttpGet("categories")]
    [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
    public IActionResult GetCategories() => Ok(UnitCatalog.Categories);

    /// <summary>Lists all supported units, optionally filtered by category (see /api/categories).</summary>
    [HttpGet("units")]
    [ProducesResponseType<IEnumerable<Unit>>(StatusCodes.Status200OK)]
    public IActionResult GetUnits([FromQuery] string? category = null)
    {
        var units = category is null
            ? UnitCatalog.Units
            : UnitCatalog.Units.Where(u => u.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        return Ok(units);
    }
}
