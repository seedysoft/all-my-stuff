using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Support.Extensions;
using Seedysoft.Libs.Utils.Extensions;

namespace Seedysoft.WebComparer.Lib.Services;

public sealed class WebComparerHostedService(IServiceProvider serviceProvider, ILogger<WebComparerHostedService> logger) : Microsoft.Extensions.Hosting.IHostedService
{
    private static readonly TimeSpan FiveSecondsTimeSpan = TimeSpan.FromSeconds(5);
    private static readonly CancellationTokenSource cancellationTokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Called {ApplicationName} version {Version}", GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        _ = await Task.Factory.StartNew(() => InfiniteMethodAsync(cancellationToken), cancellationTokenSource.Token);
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationTokenSource.Cancel();

        logger.LogInformation("End {ApplicationName}", GetType().FullName);

        await Task.CompletedTask;
    }

    private async Task InfiniteMethodAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await FindDifferencesAsync(cancellationToken);

            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }

    public async Task FindDifferencesAsync(CancellationToken cancellationToken)
    {
        Libs.Infrastructure.DbContexts.DbCxt dbCtx = serviceProvider.GetRequiredService<Libs.Infrastructure.DbContexts.DbCxt>();

        try
        {
            IQueryable<Libs.Core.Entities.WebData> WebDatasWithSubscribers =
                from w in dbCtx.WebDatas
                join s in dbCtx.Subscriptions on w.SubscriptionId equals s.SubscriptionId
                where s.Subscribers.Count != 0
                select w;
            Libs.Core.Entities.WebData[] WebDatas = await WebDatasWithSubscribers
                .Distinct()
                .ToArrayAsync(cancellationToken);
            logger.LogInformation("Obtained {WebDatas} URLs to check", WebDatas.Length);

            for (int i = 0; i < WebDatas.Length; i++)
            {
                Libs.Core.Entities.WebData webData = WebDatas[i];

                try
                {
                    await FindDataToSendAsync(dbCtx, webData, cancellationToken);

                    if (!System.Diagnostics.Debugger.IsAttached)
                        await Task.Delay(FiveSecondsTimeSpan, cancellationToken);
                }
                catch (TaskCanceledException e) when (e.InnerException is TimeoutException && logger.LogAndHandle(e.InnerException, "Request to '{WebUrl}' timeout", webData.WebUrl)) { continue; }
                catch (TaskCanceledException e) when (logger.LogAndHandle(e, "Task request to '{WebUrl}' cancelled", webData.WebUrl)) { continue; }
                catch (Exception e) when (logger.LogAndHandle(e, "Request to '{WebUrl}' failed", webData.WebUrl)) { continue; }
            }
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }
    }

    private async Task FindDataToSendAsync(Libs.Infrastructure.DbContexts.DbCxt dbCtx, Libs.Core.Entities.WebData webData, CancellationToken cancellationToken)
    {
        string Content = GetContent(webData);

        webData.DataToSend = GetDifferencesOrNull(webData, Content);

        // Establecemos la misma hora, para saber cuándo se ha visto el último cambio.
        if (!string.IsNullOrWhiteSpace(webData.DataToSend))
        {
            webData.UpdatedAtDateTimeOffset = webData.SeenAtDateTimeOffset;

            Libs.Core.Entities.Outbox Message = new(
                Libs.Core.Enums.SubscriptionName.webComparer,
                $"Ha cambiado '{webData.Hyperlink}'.{Environment.NewLine}{webData.DataToSend}")
            {
                SubscriptionId = webData.SubscriptionId
            };

            _ = await dbCtx.Outbox.AddAsync(Message, cancellationToken);
        }

        _ = await dbCtx.SaveChangesAsync(cancellationToken);
    }

    private string GetContent(Libs.Core.Entities.WebData webData)
    {
        string Content = default!;

        if (webData.UseHttpClient)
        {
            HtmlAgilityPack.HtmlWeb htmlWeb = new()
            {
                UseCookies = true,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",
                UsingCache = false,
            };
            HtmlAgilityPack.HtmlDocument htmlDocument = htmlWeb.Load(webData.WebUrl);

            Content = htmlDocument.DocumentNode.SelectSingleNode("//body").InnerText;
        }
        else
        {
            try
            {
                using OpenQA.Selenium.Chrome.ChromeDriver WebDriver = GetWebDriver();
                {
                    WebDriver.Navigate().GoToUrl(webData.WebUrl);

                    TryPerformWebDriverActions(WebDriver);

                    Content = WebDriver.FindElement(OpenQA.Selenium.By.CssSelector(webData.CssSelector)).Text;

                    WebDriver.Quit();
                }
            }
            catch (Exception e) when (logger.LogAndHandle(e, "GetContent failed with ChromeDriver for '{WebUrl}'", webData.WebUrl))
            {
                Content = default!;
            }
        }

        return Content;
    }

    private static OpenQA.Selenium.Chrome.ChromeDriver GetWebDriver()
    {
        OpenQA.Selenium.Chrome.ChromeOptions Options = new()
        {
            AcceptInsecureCertificates = true,
            BrowserVersion = "stable",
            LeaveBrowserRunning = false,
        };
        //Options.AddArgument("start-maximized");
        //Options.AddArgument("disable-infobars");
        //Options.AddArgument("--disable-extensions");
        //Options.AddArgument("--disable-dev-shm-usage");
        //Options.AddArgument("--no-sandbox");
        Options.AddArgument("--headless");

        OpenQA.Selenium.Chrome.ChromeDriver WebDriver;
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
        {
            Options.BinaryLocation = "/usr/lib/chromium-browser/chromium-browser";

            var Service = OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService("/usr/lib/chromium-browser/", "chromedriver");

            WebDriver = new(Service, Options);
        }
        else
        {
            WebDriver = new(Options);
        }

        // TODO Add TimeoutsTimeSpan setting (best for each website?)
        var TimeoutsTimeSpan = TimeSpan.FromMinutes(2);
        WebDriver.Manage().Timeouts().AsynchronousJavaScript = TimeoutsTimeSpan;
        WebDriver.Manage().Timeouts().ImplicitWait = TimeoutsTimeSpan;
        WebDriver.Manage().Timeouts().PageLoad = TimeoutsTimeSpan;

        return WebDriver;
    }

    private void TryPerformWebDriverActions(OpenQA.Selenium.Chrome.ChromeDriver webDriver)
    {
        int RetryCount = 2;

        do
        {
            // TODO Personalizar cada web con lo que podemos hacer antes de obtener datos
            try
            {
                // SubscriptionId: 3 JCyL Convocatorias
                if (webDriver.PageSource.Contains("elemento-invisible"))
                    webDriver.ExecuteJavaScript("jQuery('.elemento-invisible').removeClass('elemento-invisible');");

                // SubscriptionId: 20 Inscripción en Pruebas Selectivas
                if (webDriver.PageSource.Contains("view-more-link"))
                    ((OpenQA.Selenium.IWebElement?)webDriver.FindElement(OpenQA.Selenium.By.Id("view-more-link")))?.Click();

                RetryCount = 0;
            }
            catch (Exception e) when (logger.LogAndHandle(e, "{TryToPerformWebDriverActions} failed", nameof(TryPerformWebDriverActions))) { RetryCount--; }
        }
        while (RetryCount > 0);
    }

    private static string? GetDifferencesOrNull(Libs.Core.Entities.WebData webData, string obtainedString)
    {
        if (string.IsNullOrWhiteSpace(obtainedString))
            return null;

        string ObtainedTextNormalized = NormalizeObtainedText(obtainedString);

        if (string.IsNullOrWhiteSpace(ObtainedTextNormalized))
            return null;

        webData.SeenAtDateTimeOffset = DateTimeOffset.Now;

        DiffPlex.DiffBuilder.Model.DiffPaneModel DiffModel = DiffPlex.DiffBuilder.InlineDiffBuilder.Diff(
            oldText: webData.CurrentWebContent ?? string.Empty,
            newText: ObtainedTextNormalized,
            ignoreWhiteSpace: true,
            ignoreCase: true);

        if (ShouldIgnoreChanges(DiffModel, webData))
            return null;

        webData.CurrentWebContent = ObtainedTextNormalized;

        Dictionary<int, string> DataForMail = GetDataForMail(webData, DiffModel);

        return string.Join(Environment.NewLine, DataForMail.OrderBy(x => x.Key).Select(x => x.Value));
    }

    private static Dictionary<int, string> GetDataForMail(Libs.Core.Entities.WebData webData, DiffPlex.DiffBuilder.Model.DiffPaneModel DiffModel)
    {
        Dictionary<int, string> DataForMail = new(DiffModel.Lines.Count);

        for (int RealLineIndex = 0; RealLineIndex < DiffModel.Lines.Count; RealLineIndex++)
        {
            DiffPlex.DiffBuilder.Model.DiffPiece CurrentLine = DiffModel.Lines[RealLineIndex];

            switch (CurrentLine.Type)
            {
                case DiffPlex.DiffBuilder.Model.ChangeType.Deleted:
                case DiffPlex.DiffBuilder.Model.ChangeType.Inserted:
                case DiffPlex.DiffBuilder.Model.ChangeType.Modified:
                    // Add Current Line
                    _ = DataForMail.TryAdd(RealLineIndex, $"{GetChangeTypePreffix(CurrentLine.Type)}{CurrentLine.Text}");

                    // Then add X previous and X next lines.
                    int indexToAdd;
                    DiffPlex.DiffBuilder.Model.DiffPiece diffPiece;
                    for (int j = 1; j <= webData.TakeAboveBelowLines; j++)
                    {
                        indexToAdd = Math.Max(RealLineIndex - j, 0);
                        diffPiece = DiffModel.Lines[indexToAdd];
                        _ = DataForMail.TryAdd(indexToAdd, $"{GetChangeTypePreffix(diffPiece.Type)}{diffPiece.Text}");

                        indexToAdd = Math.Min(RealLineIndex + j, DiffModel.Lines.Count - 1);
                        diffPiece = DiffModel.Lines[indexToAdd];
                        _ = DataForMail.TryAdd(indexToAdd, $"{GetChangeTypePreffix(diffPiece.Type)}{diffPiece.Text}");
                    };
                    break;

                case DiffPlex.DiffBuilder.Model.ChangeType.Unchanged:
                case DiffPlex.DiffBuilder.Model.ChangeType.Imaginary:
                default:
                    break;
            }
        }

        return DataForMail;
    }

    private static string NormalizeObtainedText(string obtainedString)
    {
        IEnumerable<string> ObtainedLinesNormalized =
            from l in DiffPlex.Chunkers.LineChunker.Instance.Chunk(obtainedString)
            let nl = NormalizeTextLine(l)
            where !string.IsNullOrWhiteSpace(nl)
            select nl;

        // Subscriptions: Ayto Burgos y Dip Burgos Tablones
        if (ObtainedLinesNormalized.ElementAtOrDefault(0)?.StartsWith("Descripción Tablón Fecha") ?? false)
            ObtainedLinesNormalized = ObtainedLinesNormalized.Take(1).Union(ObtainedLinesNormalized.Where(x => x.EndsWith("Personal")));

        return string.Join(Environment.NewLine, ObtainedLinesNormalized);
    }

    private static string NormalizeTextLine(string text)
    {
        return text
            .Replace("\r\n", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\t", string.Empty)
            .Trim();
    }

    private static bool ShouldIgnoreChanges(DiffPlex.DiffBuilder.Model.DiffPaneModel diffModel, Libs.Core.Entities.WebData webData)
    {
        if (!diffModel.HasDifferences)
            return true;

        DiffPlex.DiffBuilder.Model.DiffPiece[] ChangedLines = diffModel.Lines.Where(x => x.Type != DiffPlex.DiffBuilder.Model.ChangeType.Unchanged).ToArray();
        string[]? IgnoreTexts = webData.IgnoreChangeWhen?.Split(';', StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < IgnoreTexts?.Length; j++)
        {
            for (int i = 0; i < ChangedLines.Length; i++)
            {
                if (ChangedLines[i].Text.Contains(IgnoreTexts[j]))
                    return true;
            }
        }

        return false;
    }

    private static string GetChangeTypePreffix(DiffPlex.DiffBuilder.Model.ChangeType lineChangeType)
    {
        return lineChangeType switch
        {
            DiffPlex.DiffBuilder.Model.ChangeType.Deleted => "-  ",
            DiffPlex.DiffBuilder.Model.ChangeType.Inserted => "+  ",
            DiffPlex.DiffBuilder.Model.ChangeType.Modified => "~  ",
            DiffPlex.DiffBuilder.Model.ChangeType.Unchanged => "=  ",
            DiffPlex.DiffBuilder.Model.ChangeType.Imaginary or _ => string.Empty,
        };
    }
}
