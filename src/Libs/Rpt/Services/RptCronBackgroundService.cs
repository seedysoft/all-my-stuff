using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Seedysoft.Libs.Core.Extensions;
using System.Collections.Frozen;

namespace Seedysoft.Libs.Rpt.Services;

public sealed class RptCronBackgroundService : BackgroundServices.Cron
{
    // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
    private static readonly HttpClient client = new();

    private readonly ILogger<RptCronBackgroundService> Logger;
    private Settings.RptSettings RptSettings => (Settings.RptSettings)Config;

    public RptCronBackgroundService(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        : base(serviceProvider, hostApplicationLifetime)
    {
        Config = ServiceProvider.GetRequiredService<IConfiguration>().GetSection(nameof(Settings.RptSettings)).Get<Settings.RptSettings>()!;

        Logger = ServiceProvider.GetRequiredService<ILogger<RptCronBackgroundService>>();
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        if (System.Diagnostics.Debugger.IsAttached)
            System.Diagnostics.Debugger.Break();

        string? AppName = GetType().FullName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        try
        {
            //await ObtainDataAsync(cancellationToken);

            await ParseFilesAsync(cancellationToken);

            //Logger.LogInformation($"Updating result: {UpgradeResult}");
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    private async Task ObtainDataAsync(CancellationToken cancellationToken)
    {
        var FileOptions = new FileStreamOptions()
        {
            Access = FileAccess.ReadWrite,
            Mode = FileMode.Create,
            Options = System.IO.FileOptions.Asynchronous,
            Share = FileShare.Write
        };
        _ = Directory.CreateDirectory("Temp");

        // 1. Obtener todos los enlaces
        var HtmlWeb = new HtmlAgilityPack.HtmlWeb();
        HtmlAgilityPack.HtmlDocument topHtmlDocument = await HtmlWeb.LoadFromWebAsync(RptSettings.Url, cancellationToken);
        var topHrefs = topHtmlDocument.DocumentNode.SelectNodes("//a[@href]")
            .Select(static x => x.GetAttributeValue("href", string.Empty))
            .Where(static x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith('#') && x.Contains("RPT") && x.EndsWith(".html"))
            .Distinct()
            .ToFrozenSet();

        if (cancellationToken.IsCancellationRequested)
            return;

        for (int i = 0; i < topHrefs.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            // For each page, load and obtain files.
            string pageUrl = topHrefs.ElementAt(i);
            string pageAbsoluteUri = new Uri(new Uri(RptSettings.Url), pageUrl).AbsoluteUri;
            HtmlAgilityPack.HtmlDocument pageHtmlDocument = await HtmlWeb.LoadFromWebAsync(pageAbsoluteUri, cancellationToken);
            var PageRefs = pageHtmlDocument.DocumentNode.SelectNodes("//a[@href]")
                .Select(static x => x.GetAttributeValue("href", string.Empty))
                .Where(static x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith('#') && x.Contains("PF") && (x.EndsWith("pdf") || x.EndsWith("xlsx")))
                .Distinct()
                .ToFrozenSet();

            for (int j = 0; j < PageRefs.Count; j++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                string fileRef = PageRefs.ElementAt(j);
                var fileUri = new Uri(new Uri(RptSettings.Url), fileRef);
                using FileStream fileStream = new($"Temp{Path.DirectorySeparatorChar}{fileRef[(fileRef.LastIndexOf('/') + 1)..]}", FileOptions);
                using Stream stream = await client.GetStreamAsync(fileUri, cancellationToken);
                await stream.CopyToAsync(fileStream, cancellationToken);
            }
        }
    }

    private async Task ParseFilesAsync(CancellationToken cancellationToken)
    {
        //IEnumerable<string> PdfFileNames = Directory.EnumerateFiles("Temp", "*.pdf", SearchOption.TopDirectoryOnly);
        //foreach (string PdfFileName in PdfFileNames)
        //    await TryToParsePdfAsync(PdfFileName, cancellationToken);

        IEnumerable<string> ExcelFileNames = Directory.EnumerateFiles("Temp", "*.xlsx", SearchOption.TopDirectoryOnly);
        foreach (string ExcelFileName in ExcelFileNames)
            await TryToParseExcelAsync(ExcelFileName, cancellationToken);
    }

    private async Task TryToParsePdfAsync(string fullFilePath, CancellationToken cancellationToken)
    {
        try
        {
            PdfSharp.Pdf.PdfDocument doc = PdfSharp.Pdf.IO.PdfReader.Open(fullFilePath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
            foreach (PdfSharp.Pdf.PdfPage page in doc.Pages)
            {
                foreach (PdfSharp.Pdf.Advanced.PdfContent content in page.Contents)
                {
                    int i = 0;
                    _ = System.Text.Encoding.Latin1.GetString(content.Stream.Value);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error parsing Pdf");
        }

        await Task.CompletedTask;
    }

    private async Task TryToParseExcelAsync(string fullFilePath, CancellationToken cancellationToken)
    {
        try
        {
            using Infrastructure.DbContexts.DbRpt ctx = ServiceProvider.GetRequiredService<Infrastructure.DbContexts.DbRpt>();
            NPOI.XSSF.UserModel.XSSFWorkbook xssWorkbook;
            using (var stream = new FileStream(fullFilePath, FileMode.Open))
                xssWorkbook = new(stream);

            NPOI.SS.UserModel.ISheet sheet = xssWorkbook.GetSheetAt(0);
            //NPOI.SS.UserModel.IRow headerRow = sheet.GetRow(0);
            //int cellCount = headerRow.LastCellNum;
            //for (int j = 0; j < cellCount; j++)
            //{
            //    NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
            //    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
            //        continue;
            //}

            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
                if (row == null || !int.TryParse(row.GetCell(0).StringCellValue, out int _))
                    continue;

                //for (int j = row.FirstCellNum; j < cellCount; j++)
                //{
                //    NPOI.SS.UserModel.ICell cell = row.GetCell(j);
                //    if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                //    {

                //    }
                //}

                Core.Entities.Puesto puesto = await GetOrAddPuestoAsync(ctx, row, cancellationToken);
                _ = await ctx.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Error parsing Excel");
        }

        await Task.CompletedTask;
    }

    private static async Task<Core.Entities.Puesto> GetOrAddPuestoAsync(
        Infrastructure.DbContexts.DbRpt ctx
        , NPOI.SS.UserModel.IRow row
        , CancellationToken cancellationToken)
    {
        Core.Entities.Pais paisUnidad = await GetOrAddPaisAsync(ctx, int.Parse(row.GetCell((int)Constants.ExcelColumns.UnidadPaisId).StringCellValue), row.GetCell((int)Constants.ExcelColumns.UnidadPaisDenominacion).StringCellValue, cancellationToken);
        Core.Entities.Provincia provinciaUnidad = await GetOrAddProvinciaAsync(ctx, int.Parse(row.GetCell((int)Constants.ExcelColumns.UnidadProvinciaId).StringCellValue), row.GetCell((int)Constants.ExcelColumns.UnidadProvinciaDenominacion).StringCellValue, paisUnidad.PaisId, cancellationToken);
        _ = await GetOrAddLocalidadAsync(ctx, int.Parse(row.GetCell((int)Constants.ExcelColumns.UnidadLocalidadId).StringCellValue), row.GetCell((int)Constants.ExcelColumns.UnidadLocalidadDenominacion).StringCellValue, provinciaUnidad.ProvinciaId, provinciaUnidad.PaisId, cancellationToken);
        _ = await GetOrAddMinisterioAsync(ctx, row, cancellationToken);
        _ = await GetOrAddCentroDirectivoAsync(ctx, row, cancellationToken);
        Core.Entities.Unidad unidad = await GetOrAddUnidadAsync(ctx, row, cancellationToken);

        int idPuesto = int.Parse(row.GetCell((int)Constants.ExcelColumns.Puesto).StringCellValue);

        Core.Entities.Puesto? puesto = await ctx.Puestos
            .FirstOrDefaultAsync(x => x.PuestoId == idPuesto, cancellationToken);

        if (puesto == null)
        {
            puesto = new Core.Entities.Puesto()
            {
                PuestoId = idPuesto,
                ComplementoEspecifico = Convert.ToDecimal(row.GetCell((int)Constants.ExcelColumns.ComplementoEspecifico).NumericCellValue, Core.Constants.Globalization.NumberFormatInfoES),
                Estado = row.GetCell((int)Constants.ExcelColumns.Estado).StringCellValue,
                Nivel = int.Parse(row.GetCell((int)Constants.ExcelColumns.Nivel).StringCellValue),
                PuestoDenominacionCorta = row.GetCell((int)Constants.ExcelColumns.PuestoDenominacionCorta).StringCellValue,
                PuestoDenominacionLarga = row.GetCell((int)Constants.ExcelColumns.PuestoDenominacionLarga).StringCellValue,
                UnidadId = unidad.UnidadId,
            };
            _ = await ctx.AddAsync(puesto, cancellationToken);
        }

        // Nullable values
        puesto.Adscripcion = row.GetCell((int)Constants.ExcelColumns.AdscripcionPuesto).StringCellValue;
        puesto.Cuerpo = row.GetCell((int)Constants.ExcelColumns.AgrupacionCuerpo).StringCellValue;
        puesto.FormacionEspecifica = row.GetCell((int)Constants.ExcelColumns.FormacionEspecifica).StringCellValue;
        puesto.GrupoSubgrupo = row.GetCell((int)Constants.ExcelColumns.GrupoSubgrupo).StringCellValue;
        puesto.Observaciones = row.GetCell((int)Constants.ExcelColumns.Observaciones).StringCellValue;
        puesto.Provision = row.GetCell((int)Constants.ExcelColumns.Provision).StringCellValue;
        puesto.TipoPuesto = row.GetCell((int)Constants.ExcelColumns.TipoPuesto).StringCellValue;
        puesto.TitulacionAcademica = row.GetCell((int)Constants.ExcelColumns.TitulacionAcademica).StringCellValue;

        // Nullable location
        if (int.TryParse(row.GetCell((int)Constants.ExcelColumns.PaisId).StringCellValue, out int Dummy))
        {
            puesto.PaisId = Dummy;
            _ = await GetOrAddPaisAsync(ctx, Dummy, row.GetCell((int)Constants.ExcelColumns.PaisDenominacion).StringCellValue, cancellationToken);
        }
        if (int.TryParse(row.GetCell((int)Constants.ExcelColumns.ProvinciaId).StringCellValue, out Dummy))
        {
            puesto.ProvinciaId = Dummy;
            _ = await GetOrAddProvinciaAsync(ctx, Dummy, row.GetCell((int)Constants.ExcelColumns.ProvinciaDenominacion).StringCellValue, puesto.PaisId.GetValueOrDefault(), cancellationToken);
        }
        if (int.TryParse(row.GetCell((int)Constants.ExcelColumns.LocalidadId).StringCellValue, out Dummy))
        {
            puesto.LocalidadId = Dummy;
            _ = await GetOrAddLocalidadAsync(ctx, Dummy, row.GetCell((int)Constants.ExcelColumns.LocalidadDenominacion).StringCellValue, puesto.ProvinciaId.GetValueOrDefault(), puesto.PaisId.GetValueOrDefault(), cancellationToken);
        }

        return puesto;
    }

    private static async Task<Core.Entities.Pais> GetOrAddPaisAsync(
        Infrastructure.DbContexts.DbRpt ctx
        , int idPais
        , string denominacionPais
        , CancellationToken cancellationToken)
    {
        Core.Entities.Pais? pais = await ctx.Paises
            .FirstOrDefaultAsync(x => x.PaisId == idPais, cancellationToken);

        if (pais == null)
        {
            pais = new Core.Entities.Pais()
            {
                PaisId = idPais,
                PaisDenominacion = denominacionPais,
            };
            _ = await ctx.AddAsync(pais, cancellationToken);
            _ = await ctx.SaveChangesAsync(cancellationToken);
        }

        return pais;
    }
    private static async Task<Core.Entities.Provincia> GetOrAddProvinciaAsync(
        Infrastructure.DbContexts.DbRpt ctx
        , int idProvincia
        , string denominacionProvincia
        , int idPais
        , CancellationToken cancellationToken)
    {
        Core.Entities.Provincia? provincia = await ctx.Provincias
            .FirstOrDefaultAsync(x => x.PaisId == idPais && x.ProvinciaId == idProvincia, cancellationToken);

        if (provincia == null)
        {
            provincia = new Core.Entities.Provincia()
            {
                ProvinciaId = idProvincia,
                ProvinciaDenominacion = denominacionProvincia,
                PaisId = idPais,
            };
            _ = await ctx.AddAsync(provincia, cancellationToken);
            _ = await ctx.SaveChangesAsync(cancellationToken);
        }

        return provincia;
    }
    private static async Task<Core.Entities.Localidad> GetOrAddLocalidadAsync(
        Infrastructure.DbContexts.DbRpt ctx
        , int idLocalidad
        , string denominacionLocalidad
        , int idProvincia
        , int idPais
        , CancellationToken cancellationToken)
    {
        Core.Entities.Localidad? localidad = await ctx.Localidades
            .FirstOrDefaultAsync(x => x.PaisId == idPais && x.ProvinciaId == idProvincia && x.LocalidadId == idLocalidad, cancellationToken);

        if (localidad == null)
        {
            localidad = new Core.Entities.Localidad()
            {
                LocalidadId = idLocalidad,
                LocalidadDenominacion = denominacionLocalidad,
                ProvinciaId = idProvincia,
                PaisId = idPais,
            };
            _ = await ctx.AddAsync(localidad, cancellationToken);
            _ = await ctx.SaveChangesAsync(cancellationToken);
        }

        return localidad;
    }

    private static async Task<Core.Entities.Ministerio> GetOrAddMinisterioAsync(
        Infrastructure.DbContexts.DbRpt ctx
        , NPOI.SS.UserModel.IRow row
        , CancellationToken cancellationToken)
    {
        int idMinisterio = int.Parse(row.GetCell((int)Constants.ExcelColumns.MinisterioId).StringCellValue);

        Core.Entities.Ministerio? ministerio = await ctx.Ministerios
            .FirstOrDefaultAsync(x => x.MinisterioId == idMinisterio, cancellationToken);

        if (ministerio == null)
        {
            ministerio = new Core.Entities.Ministerio()
            {
                MinisterioId = idMinisterio,
                MinisterioDenominacion = row.GetCell((int)Constants.ExcelColumns.MinisterioDenominacion).StringCellValue
            };
            _ = await ctx.AddAsync(ministerio, cancellationToken);
            _ = await ctx.SaveChangesAsync(cancellationToken);
        }

        return ministerio;
    }

    private static async Task<Core.Entities.CentroDirectivo> GetOrAddCentroDirectivoAsync(
        Infrastructure.DbContexts.DbRpt ctx
        , NPOI.SS.UserModel.IRow row
        , CancellationToken cancellationToken)
    {
        int idCentroDirectivo = int.Parse(row.GetCell((int)Constants.ExcelColumns.CentroDirectivoId).StringCellValue);

        Core.Entities.CentroDirectivo? centroDirectivo = await ctx.CentrosDirectivos
            .FirstOrDefaultAsync(x => x.CentroDirectivoId == idCentroDirectivo, cancellationToken);

        if (centroDirectivo == null)
        {
            centroDirectivo = new Core.Entities.CentroDirectivo()
            {
                CentroDirectivoId = idCentroDirectivo,
                CentroDirectivoDenominacion = row.GetCell((int)Constants.ExcelColumns.CentroDirectivoDenominacion).StringCellValue,
                MinisterioId = int.Parse(row.GetCell((int)Constants.ExcelColumns.MinisterioId).StringCellValue),
            };
            _ = await ctx.AddAsync(centroDirectivo, cancellationToken);
            _ = await ctx.SaveChangesAsync(cancellationToken);
        }

        return centroDirectivo;
    }

    private static async Task<Core.Entities.Unidad> GetOrAddUnidadAsync(
        Infrastructure.DbContexts.DbRpt ctx
        , NPOI.SS.UserModel.IRow row
        , CancellationToken cancellationToken)
    {
        int idUnidad = int.Parse(row.GetCell((int)Constants.ExcelColumns.UnidadId).StringCellValue);

        Core.Entities.Unidad? unidad = await ctx.Unidades
            .FirstOrDefaultAsync(e => e.UnidadId == idUnidad, cancellationToken);

        if (unidad == null)
        {
            unidad = new Core.Entities.Unidad()
            {
                UnidadId = idUnidad,
                UnidadDenominacion = row.GetCell((int)Constants.ExcelColumns.UnidadDenominacion).StringCellValue,
                CentroDirectivoId = int.Parse(row.GetCell((int)Constants.ExcelColumns.CentroDirectivoId).StringCellValue),
                LocalidadId = int.Parse(row.GetCell((int)Constants.ExcelColumns.UnidadLocalidadId).StringCellValue),
                PaisId = int.Parse(row.GetCell((int)Constants.ExcelColumns.UnidadPaisId).StringCellValue),
                ProvinciaId = int.Parse(row.GetCell((int)Constants.ExcelColumns.UnidadProvinciaId).StringCellValue),
            };
            _ = await ctx.AddAsync(unidad, cancellationToken);
            _ = await ctx.SaveChangesAsync(cancellationToken);
        }

        return unidad;
    }
}
