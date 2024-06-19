using System.ComponentModel.DataAnnotations;

namespace Seedysoft.BlazorWebApp.Client.ViewModels;

public record GasStationInfoModel
{
    public int IdEstacion { get; set; }

    public string Estacion { get; set; } = default!;

    public string Latitud { get; set; } = default!;

    public string Localidad { get; set; } = default!;

    public string Longitud { get; set; } = default!;

    public string Municipio { get; set; } = default!;

    [DisplayFormat(DataFormatString = "{0:0.000;;}")]
    public decimal Precio { get; set; }

    public string Producto { get; set; } = default!;

    public string Provincia { get; set; } = default!;

    // TODO     Usar la ruta seleccionada y mostrar el punto
    public string GoogleMapsUri => $"https://www.google.com/maps/place/{Latitud.Replace(",", ".")},{Longitud.Replace(",", ".")}/";
}
