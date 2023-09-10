using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Seedysoft.UtilsLib.Extensions;
using System.Collections.Immutable;

namespace Seedysoft.WebComparerLib;

public static class Main
{
    public static async Task FindDifferencesAsync(DbContexts.DbCxt dbCtx, ILogger logger)
    {
        try
        {
            IQueryable<Entities.WebData> WebDatasWithSubscribers =
                from w in dbCtx.WebDatas
                join s in dbCtx.Subscriptions on w.SubscriptionId equals s.SubscriptionId
                where s.Subscribers.Any()
                select w;
            Entities.WebData[] WebDatas = await WebDatasWithSubscribers.Distinct().ToArrayAsync();
            logger.LogInformation("Obtained {WebDatas} URLs to check", WebDatas.Length);

            using IWebDriver WebDriver = GetWebDriver(logger);

            for (int i = 0; i < WebDatas.Length; i++)
            {
                Entities.WebData webData = WebDatas[i];

                try
                {
                    WebDriver.Navigate().GoToUrl(webData.WebUrl);

                    string OnlyBodyStriped = GetContent(WebDriver, webData, logger);

                    webData.DataToSend = FindDifferences(webData, OnlyBodyStriped);

                    // Establecemos la misma hora, para saber cuándo se ha visto el último cambio.
                    if (!string.IsNullOrWhiteSpace(webData.DataToSend))
                    {
                        webData.UpdatedAtDateTimeOffset = webData.SeenAtDateTimeOffset;

                        var Message = new CoreLib.Entities.Outbox(
                            CoreLib.Enums.SubscriptionName.webComparer,
                            $"Ha cambiado '{webData.Hyperlink}'.{Environment.NewLine}{webData.DataToSend}")
                        {
                            SubscriptionId = webData.SubscriptionId
                        };

                        _ = await dbCtx.Outbox.AddAsync(Message);
                    }

                    _ = await dbCtx.SaveChangesAsync();
                }
                catch (TaskCanceledException e) when (e.InnerException is TimeoutException && logger.LogAndHandle(e.InnerException, "Request to '{WebUrl}' timeout", webData.WebUrl)) { continue; }
                catch (TaskCanceledException e) when (logger.LogAndHandle(e, "Task request to '{WebUrl}' cancelled", webData.WebUrl)) { continue; }
                catch (Exception e) when (logger.LogAndHandle(e, "Request to '{WebUrl}' failed", webData.WebUrl)) { continue; }
            }

            WebDriver.Quit();
        }
        catch (Exception e) when (logger.LogAndHandle(e, "Unexpected error")) { }
        finally { await Task.CompletedTask; }
    }

    private static IWebDriver GetWebDriver(ILogger logger)
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

        //logger.LogInformation("Current OSDescription: '{OSDescription}'", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
        //logger.LogInformation("Current RuntimeIdentifier: '{RuntimeIdentifier}'", System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier);
        //logger.LogInformation("Current ProcessArchitecture: '{ProcessArchitecture}'", System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture);
        //logger.LogInformation("Current Platform: '{Platform}'", Environment.OSVersion.Platform);

        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            Options.BinaryLocation = "/usr/bin/chromium-browser";
        //else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        //    Options.BinaryLocation = Environment.CurrentDirectory;
        logger.LogInformation("Options Binary location: '{BinaryLocation}'", Options.BinaryLocation);

        IWebDriver WebDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Options);
        var OneMinuteTimeSpan = TimeSpan.FromMinutes(1);
        WebDriver.Manage().Timeouts().AsynchronousJavaScript = OneMinuteTimeSpan;
        WebDriver.Manage().Timeouts().ImplicitWait = OneMinuteTimeSpan;
        WebDriver.Manage().Timeouts().PageLoad = OneMinuteTimeSpan;

        return WebDriver;
    }

    private static string GetContent(IWebDriver webDriver, Entities.WebData webData, ILogger logger)
    {
        // TODO         Add retry logic when timeout
        // TODO Personalizar cada web con lo que podemos hacer antes de obtener datos

        WebDriverWait WaitDriver = new(webDriver, timeout: TimeSpan.FromSeconds(30))
        {
            PollingInterval = TimeSpan.FromSeconds(5),
        };
        WaitDriver.IgnoreExceptionTypes(typeof(NoSuchElementException));

        // SubscriptionId: 20 Inscripción en Pruebas Selectivas
        if (webDriver.PageSource.Contains("view-more-link"))
            ((IWebElement?)webDriver.FindElement(By.Id("view-more-link")))?.Click();

        // SubscriptionId: 29 Ayto Burgos Tablón de anuncios
        if (webDriver.PageSource.Contains("remitenteFiltro"))
        {
            IWebElement RemitenteFiltroWebElement = WaitDriver.Until(drv => drv.FindElement(By.Id("remitenteFiltro")));

            new SelectElement(RemitenteFiltroWebElement).SelectByText("Personal");

            webDriver.FindElement(By.CssSelector(".botonera a.primary")).Click();

            _ = WaitDriver.Until(drv => drv.FindElements(By.CssSelector("body.waiting")).Count == 0);
        }

        logger.LogDebug("Antes de Until en '{WebUrl}'", webData.WebUrl);
        IWebElement Element = WaitDriver.Until(x => x.FindElement(By.CssSelector(webData.CssSelector)));
        logger.LogDebug("Después de Until");

        try
        {
            return Element.Text;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Cannot load {cssSelector} on first try", webData.CssSelector);

            return webDriver.FindElement(By.CssSelector(webData.CssSelector)).Text;
        }
    }

    private static string FindDifferences(Entities.WebData webData, string obtainedString)
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
