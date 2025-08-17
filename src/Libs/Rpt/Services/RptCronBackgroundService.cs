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
            //await ObtainDataAsync();

            await ParseFilesAsync();

            //Logger.LogInformation($"Updating result: {UpgradeResult}");
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    private async Task ObtainDataAsync()
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
        HtmlAgilityPack.HtmlDocument topHtmlDocument = await HtmlWeb.LoadFromWebAsync(RptSettings.Url);
        var topHrefs = topHtmlDocument.DocumentNode.SelectNodes("//a[@href]")
            .Select(static x => x.GetAttributeValue("href", string.Empty))
            .Where(static x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith('#') && x.Contains("RPT"))
            .Distinct()
            .ToFrozenSet();

        for (int i = 0; i < topHrefs.Count; i++)
        {
            // For each page, load and obtain files.
            string pageUrl = topHrefs.ElementAt(i);
            string pageAbsoluteUri = new Uri(new Uri(RptSettings.Url), pageUrl).AbsoluteUri;
            HtmlAgilityPack.HtmlDocument pageHtmlDocument = await HtmlWeb.LoadFromWebAsync(pageAbsoluteUri);
            var PageRefs = pageHtmlDocument.DocumentNode.SelectNodes("//a[@href]")
                .Select(static x => x.GetAttributeValue("href", string.Empty))
                .Where(static x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith('#') && (x.EndsWith("pdf") || x.EndsWith("xlsx")))
                .Distinct()
                .ToFrozenSet();
            for (int j = 0; j < PageRefs.Count; j++)
            {
                string fileRef = PageRefs.ElementAt(j);
                var fileUri = new Uri(new Uri(RptSettings.Url), fileRef);
                using FileStream fileStream = new($"Temp{Path.DirectorySeparatorChar}{fileRef[(fileRef.LastIndexOf('/') + 1)..]}", FileOptions);
                using Stream stream = await client.GetStreamAsync(fileUri);
                await stream.CopyToAsync(fileStream);
            }
        }
    }

    private async Task ParseFilesAsync()
    {
        IEnumerable<string> PdfFileNames = Directory.EnumerateFiles("Temp", "*.pdf", SearchOption.TopDirectoryOnly);
        foreach (string PdfFileName in PdfFileNames)
            await ParsePdfAsync(PdfFileName);

        IEnumerable<string> ExcelFileNames = Directory.EnumerateFiles("Temp", "*.xlsx", SearchOption.TopDirectoryOnly);
        foreach (string ExcelFileName in ExcelFileNames)
            await ParseExcelAsync(ExcelFileName);
    }

    private async Task ParsePdfAsync(string fullFilePath)
    {
        PdfSharp.Pdf.PdfDocument doc = PdfSharp.Pdf.IO.PdfReader.Open(fullFilePath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
        foreach (PdfSharp.Pdf.PdfPage page in doc.Pages)
        {
            foreach (PdfSharp.Pdf.Advanced.PdfContent content in page.Contents)
            {
                var i = 0;
                System.Text.Encoding.Latin1.GetString(content.Stream.Value);
            }
        }
    }

    private async Task ParseExcelAsync(string fullFilePath)
    {
        using var stream = new FileStream(fullFilePath, FileMode.Open);
        stream.Position = 0;
        NPOI.XSSF.UserModel.XSSFWorkbook xssWorkbook = new(stream);
        NPOI.SS.UserModel.ISheet sheet = xssWorkbook.GetSheetAt(0);
        NPOI.SS.UserModel.IRow headerRow = sheet.GetRow(0);
        int cellCount = headerRow.LastCellNum;
        for (int j = 0; j < cellCount; j++)
        {
            NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
            if (cell == null || string.IsNullOrWhiteSpace(cell.ToString()))
                continue;

        }

        for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
        {
            NPOI.SS.UserModel.IRow row = sheet.GetRow(i);
            if (row == null || row.Cells.All(d => d.CellType == NPOI.SS.UserModel.CellType.Blank))
                continue;

            for (int j = row.FirstCellNum; j < cellCount; j++)
            {
                if (row.GetCell(j) != null)
                {
                    if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                    {

                    }
                }
            }
        }
    }
}
