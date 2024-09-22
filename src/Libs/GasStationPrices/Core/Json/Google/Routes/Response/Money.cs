﻿namespace Seedysoft.Libs.GasStationPrices.Core.Json.Google.Routes.Response;

/// <summary>
/// Represents an amount of money with its currency type.
/// </summary>
public class Money
{
    /// <summary>
    /// The three-letter currency code defined in ISO 4217.
    /// </summary>
    [J("currencyCode")][I(Condition = C.WhenWritingNull)] public string? CurrencyCode { get; set; }
    /// <summary>
    /// The whole units of the amount. For example if currencyCode is "USD", then 1 unit is one US dollar.
    /// </summary>
    [J("units")][I(Condition = C.WhenWritingNull)] public string? Units { get; set; }
    /// <summary>
    /// Number of nano (10^-9) units of the amount. The value must be between -999,999,999 and +999,999,999 inclusive. If units is positive, nanos must be positive or zero. If units is zero, nanos can be positive, zero, or negative. If units is negative, nanos must be negative or zero. For example $-1.75 is represented as units=-1 and nanos=-750,000,000.
    /// </summary>
    [J("nanos")][I(Condition = C.WhenWritingNull)] public int? Nanos { get; set; }
}
