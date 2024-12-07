﻿namespace Seedysoft.Libs.GoogleApis.Models.Places.Response;

public record class TextAndMatches
{
    [J("text")]
    public string? Text { get; set; }

    [J("matches")]
    public Match[]? Matches { get; set; }
}
