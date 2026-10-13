# Unit Conversion API

A simple ASP.NET Core (.NET 10) Web API that converts values between units of measurement,
e.g. meters to feet, Celsius to Fahrenheit, kilograms to pounds.

Supported categories: **length, mass, temperature, volume** (20 units).

## How to run

### With .NET

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
dotnet run --project src/UnitConversion.Api
```

Open http://localhost:5220/scalar to try the API in your browser.

### With Docker

```bash
docker compose up --build
```

Open http://localhost:8080/scalar.

### Run the tests

```bash
dotnet test
```

## Endpoints

| Endpoint | Description |
| --- | --- |
| `GET /api/convert?value=10&from=m&to=ft` | Convert a value |
| `GET /api/categories` | List the supported categories |
| `GET /api/units` | List all units |
| `GET /api/units?category=temperature` | List units of one category |

Units can be given by id, symbol or alias, ignoring case: `foot`, `ft` and `feet` all work.

Example response for `GET /api/convert?value=100&from=celsius&to=fahrenheit`:

```json
{
  "category": "temperature",
  "value": 100,
  "from": { "id": "celsius", "name": "Celsius", "symbol": "°C", ... },
  "result": 212,
  "to": { "id": "fahrenheit", "name": "Fahrenheit", "symbol": "°F", ... }
}
```

Invalid requests (unknown unit, converting meters to kilograms, temperature below absolute zero,
missing value) return `400 Bad Request` with an error message.

## Project structure

```
src/
  UnitConversion.Api/      Web API: Program.cs and ConversionsController
  UnitConversion.Core/     Conversion logic
    Models/Unit.cs         A unit and its conversion factor
    UnitCatalog.cs         The list of all supported units
    UnitConverter.cs       The conversion logic
tests/
  UnitConversion.Tests/    Unit tests and API tests
```

## Design decisions

- **Conversion through a base unit.** Each category has a base unit (meter, kilogram, kelvin, liter).
  Every unit stores one factor relative to that base, and a conversion goes `from → base → to`.
  Adding a unit is one line in `UnitCatalog.cs`, with no conversion code to write. This keeps the system
  manageable as it grows to hundreds of units.
- **One formula for all units:** `base = (value + offset) × factor`. Most units have offset 0.
  Temperatures use an offset, e.g. Celsius: `K = °C + 273.15`.
- **Two projects.** The conversion logic (Core) is separate from the web layer (Api), so it can be
  tested and reused on its own.
- **Hardcoded units**, as allowed for this version. Later the list in `UnitCatalog` could be
  loaded from a database or a JSON file without changing the converter.
- **Rounding.** Results are rounded to 12 significant digits to hide floating-point noise
  (e.g. `31.9999999999999` becomes `32`).
- **GET request.** A conversion only calculates and changes nothing, so a simple GET is the natural fit,
  and it is easy to try in a browser.
