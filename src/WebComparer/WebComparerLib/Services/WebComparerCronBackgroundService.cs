﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Support.Extensions;
using Seedysoft.UtilsLib.Extensions;
using System.Collections.Immutable;

namespace Seedysoft.WebComparerLib.Services;

public class WebComparerCronBackgroundService : CronBackgroundServiceLib.CronBackgroundService
{
    private readonly IServiceProvider ServiceProvider;
    private readonly ILogger<WebComparerCronBackgroundService> Logger;

    public WebComparerCronBackgroundService(
        Settings.WebComparerSettings config
        , IServiceProvider serviceProvider
        , ILogger<WebComparerCronBackgroundService> logger) : base(config)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        string? AppName = GetType().FullName;

        Logger.LogInformation("Called {ApplicationName} version {Version}", AppName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        InfrastructureLib.DbContexts.DbCxt dbCtx = ServiceProvider.GetRequiredService<InfrastructureLib.DbContexts.DbCxt>();

        if (!System.Diagnostics.Debugger.IsAttached)
            await FindDifferencesAsync(dbCtx, cancellationToken);

        Logger.LogInformation("End {ApplicationName}", AppName);
    }

    private async Task FindDifferencesAsync(InfrastructureLib.DbContexts.DbCxt dbCtx, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<CoreLib.Entities.WebData> WebDatasWithSubscribers =
                from w in dbCtx.WebDatas
                join s in dbCtx.Subscriptions on w.SubscriptionId equals s.SubscriptionId
                where s.Subscribers.Any()
                select w;
            CoreLib.Entities.WebData[] WebDatas = await WebDatasWithSubscribers
                .Distinct()
                //.OrderByDescending(x => x.SubscriptionId)
                .ToArrayAsync(cancellationToken);
            Logger.LogInformation("Obtained {WebDatas} URLs to check", WebDatas.Length);

            using OpenQA.Selenium.IWebDriver WebDriver = GetWebDriver();
            {
                for (int i = 0; i < WebDatas.Length; i++)
                {
                    CoreLib.Entities.WebData webData = WebDatas[i];

                    try
                    {
                        await FindDataToSendAsync(dbCtx, WebDriver, webData, cancellationToken);
                    }
                    catch (TaskCanceledException e) when (e.InnerException is TimeoutException && Logger.LogAndHandle(e.InnerException, "Request to '{WebUrl}' timeout", webData.WebUrl)) { continue; }
                    catch (TaskCanceledException e) when (Logger.LogAndHandle(e, "Task request to '{WebUrl}' cancelled", webData.WebUrl)) { continue; }
                    catch (Exception e) when (Logger.LogAndHandle(e, "Request to '{WebUrl}' failed", webData.WebUrl)) { continue; }
                }

                WebDriver?.Quit();
            }
        }
        catch (Exception e) when (Logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }
    }

    private async Task FindDataToSendAsync(InfrastructureLib.DbContexts.DbCxt dbCtx, OpenQA.Selenium.IWebDriver WebDriver, CoreLib.Entities.WebData webData, CancellationToken cancellationToken)
    {
        WebDriver.Navigate().GoToUrl(webData.WebUrl);

        string ContentStriped = await GetContentAsync(WebDriver, webData);

        webData.DataToSend = GetDifferences(webData, ContentStriped);

        // Establecemos la misma hora, para saber cuándo se ha visto el último cambio.
        if (!string.IsNullOrWhiteSpace(webData.DataToSend))
        {
            webData.UpdatedAtDateTimeOffset = webData.SeenAtDateTimeOffset;

            CoreLib.Entities.Outbox Message = new(
                CoreLib.Enums.SubscriptionName.webComparer,
                $"Ha cambiado '{webData.Hyperlink}'.{Environment.NewLine}{webData.DataToSend}")
            {
                SubscriptionId = webData.SubscriptionId
            };

            _ = await dbCtx.Outbox.AddAsync(Message, cancellationToken);
        }

        _ = await dbCtx.SaveChangesAsync(cancellationToken);
    }

    private static OpenQA.Selenium.IWebDriver GetWebDriver()
    {
        OpenQA.Selenium.Chrome.ChromeOptions Options = new()
        {
            AcceptInsecureCertificates = true,
            LeaveBrowserRunning = false,
        };
        //Options.AddArgument("start-maximized");
        //Options.AddArgument("disable-infobars");
        //Options.AddArgument("--disable-extensions");
        //Options.AddArgument("--disable-dev-shm-usage");
        //Options.AddArgument("--no-sandbox");
        Options.AddArgument("--headless");

        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            Options.BinaryLocation = "/usr/bin/chromium-browser";

        OpenQA.Selenium.IWebDriver WebDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Options);

        // TODO Add TimeoutsTimeSpan setting (best for each website?)
        var TimeoutsTimeSpan = TimeSpan.FromMinutes(2);
        WebDriver.Manage().Timeouts().AsynchronousJavaScript = TimeoutsTimeSpan;
        WebDriver.Manage().Timeouts().ImplicitWait = TimeoutsTimeSpan;
        WebDriver.Manage().Timeouts().PageLoad = TimeoutsTimeSpan;

        return WebDriver;
    }

    private async Task<string> GetContentAsync(OpenQA.Selenium.IWebDriver webDriver, CoreLib.Entities.WebData webData)
    {
        // TODO         Add retry logic when timeout
        // TODO Personalizar cada web con lo que podemos hacer antes de obtener datos

        OpenQA.Selenium.Support.UI.WebDriverWait WaitDriver = new(webDriver, timeout: TimeSpan.FromSeconds(30))
        {
            PollingInterval = TimeSpan.FromSeconds(5),
        };

        // SubscriptionId: 3 JCyL Convocatorias
        if (webDriver.PageSource.Contains("elemento-invisible"))
            webDriver.ExecuteJavaScript("jQuery('.elemento-invisible').removeClass('elemento-invisible');");

        // SubscriptionId: 20 Inscripción en Pruebas Selectivas
        if (webDriver.PageSource.Contains("view-more-link"))
            ((OpenQA.Selenium.IWebElement?)webDriver.FindElement(OpenQA.Selenium.By.Id("view-more-link")))?.Click();

        // SubscriptionId: 29 Ayto Burgos Tablón de anuncios
        if (webDriver.PageSource.Contains("remitenteFiltro"))
        {
            OpenQA.Selenium.IWebElement RemitenteFiltroWebElement = WaitDriver.Until(drv => drv.FindElement(OpenQA.Selenium.By.Id("remitenteFiltro")));

            OpenQA.Selenium.Support.UI.SelectElement FilterSelectElement = new(RemitenteFiltroWebElement);
            FilterSelectElement.SelectByText("Personal");

            var FilterButtonBy = OpenQA.Selenium.By.CssSelector(".botonera a.primary");
            OpenQA.Selenium.IWebElement FilterButtonWebElement = webDriver.FindElement(FilterButtonBy);
            FilterButtonWebElement.Click();

            var BodyWaitingBy = OpenQA.Selenium.By.CssSelector("body.waiting");
            try
            {
                OpenQA.Selenium.IWebElement? BodyWaitingWebElement = webDriver.FindElement(BodyWaitingBy);

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            catch (OpenQA.Selenium.NoSuchElementException noSuchElementException)
            {
                Logger.LogDebug("Exception {noSuchElementException} finding {BodyWaitingBy}", noSuchElementException, BodyWaitingBy);
            }
            catch (OpenQA.Selenium.WebDriverException webDriverException)
            {
                Logger.LogDebug("Exception {webDriverException} finding {BodyWaitingBy}", webDriverException, BodyWaitingBy);
            }
        }

        try
        {
            OpenQA.Selenium.IWebElement Element = WaitDriver.Until(x => x.FindElement(OpenQA.Selenium.By.CssSelector(webData.CssSelector)));

            return Element.Text;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Cannot load {cssSelector} on first try", webData.CssSelector);

            return webDriver.FindElement(OpenQA.Selenium.By.CssSelector(webData.CssSelector)).Text;
        }
    }

    private static string GetDifferences(CoreLib.Entities.WebData webData, string obtainedString)
    {
        IEnumerable<string> ObtainedLinesNormalized =
            from l in DiffPlex.Chunkers.LineChunker.Instance.Chunk(obtainedString)
            let nl = NormalizeText(l)
            where !string.IsNullOrWhiteSpace(nl)
            select nl;
        string ObtainedTextNormalized = string.Join(Environment.NewLine, ObtainedLinesNormalized);

        DiffPlex.DiffBuilder.Model.DiffPaneModel DiffModel = DiffPlex.DiffBuilder.InlineDiffBuilder.Diff(
            oldText: webData.CurrentWebContent ?? string.Empty,
            newText: ObtainedTextNormalized,
            ignoreWhiteSpace: true,
            ignoreCase: true);

        // First remove empty lines and normalize non-empty
        var NormalizedDiffLines = (
            from l in DiffModel.Lines
            let nl = NormalizeText(l.Text)
            where !string.IsNullOrWhiteSpace(nl)
            select new { Line = l, NormalizedText = nl }
        ).ToImmutableArray();

        var RealChangedLines = NormalizedDiffLines
            .Select(x => new DiffPlex.DiffBuilder.Model.DiffPiece(x.NormalizedText, x.Line.Type, x.Line.Position))
            .ToImmutableArray();

        if (!string.IsNullOrWhiteSpace(webData.IgnoreChangeWhen) &&
            webData.IgnoreChangeWhen.Split(';').Any(ObtainedTextNormalized.Contains))
        {
            return string.Empty;
        }

        webData.CurrentWebContent = ObtainedTextNormalized;
        webData.SeenAtDateTimeOffset = DateTimeOffset.Now;

        int TotalTextChanged =
            RealChangedLines.Where(x => x.Type is DiffPlex.DiffBuilder.Model.ChangeType.Inserted or DiffPlex.DiffBuilder.Model.ChangeType.Modified).Select(x => x.Text.Length).Sum()
            -
            RealChangedLines.Where(x => x.Type == DiffPlex.DiffBuilder.Model.ChangeType.Deleted).Select(x => x.Text.Length).Sum();

        // TODO Parametrize minimun TotalTextChanged required for send "alarm"
        if (!DiffModel.HasDifferences || Math.Abs(TotalTextChanged) < 8)
            return string.Empty;

        Dictionary<int, string> DataForMail = new();

        for (int RealLineIndex = 0; RealLineIndex < RealChangedLines.Length; RealLineIndex++)
        {
            DiffPlex.DiffBuilder.Model.DiffPiece CurrentLine = RealChangedLines[RealLineIndex];

            switch (CurrentLine.Type)
            {
                case DiffPlex.DiffBuilder.Model.ChangeType.Deleted:
                case DiffPlex.DiffBuilder.Model.ChangeType.Inserted:
                case DiffPlex.DiffBuilder.Model.ChangeType.Modified:
                    // Add Current Line
                    _ = DataForMail.TryAdd(RealLineIndex, $"{GetPreffix(CurrentLine.Type)}{CurrentLine.Text}");

                    // Then add X previous and X next lines.
                    for (int j = 1; j <= webData.TakeAboveBelowLines; j++)
                    {
                        int IndexToAdd = Math.Max(RealLineIndex - j, 0);
                        _ = DataForMail.TryAdd(IndexToAdd, $"{GetPreffix(RealChangedLines[IndexToAdd].Type)}{RealChangedLines[IndexToAdd].Text}");

                        IndexToAdd = Math.Min(RealLineIndex + j, RealChangedLines.Length - 1);
                        _ = DataForMail.TryAdd(IndexToAdd, $"{GetPreffix(RealChangedLines[IndexToAdd].Type)}{RealChangedLines[IndexToAdd].Text}");
                    };
                    break;

                case DiffPlex.DiffBuilder.Model.ChangeType.Unchanged:
                case DiffPlex.DiffBuilder.Model.ChangeType.Imaginary:
                default:
                    break;
            }
        }

        string DataToSend = string.Join(
            Environment.NewLine,
            DataForMail.OrderBy(x => x.Key).Select(x => x.Value));

        return DataToSend;
    }

    private static string NormalizeText(string text)
    {
        return text
            .Replace("\r\n", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\t", string.Empty)
            .Trim();
    }

    private static string GetPreffix(DiffPlex.DiffBuilder.Model.ChangeType lineChangeType)
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
