namespace Seedysoft.Libs.Core.Tests;

public abstract class XUnitTestClassBase
{
    protected internal Xunit.Abstractions.ITestOutputHelper TestOutputHelper { get; }

    public XUnitTestClassBase(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;

        string NewEnvironment =
#if DEBUG
            "Development"
#else
            "Production"
#endif
;
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", NewEnvironment, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", NewEnvironment, EnvironmentVariableTarget.Process);
    }
}
