namespace Seedysoft.Libs.Core.Tests;

public abstract class TUnitTestClassBase
{
    protected internal TUnit.ITestOutputHelper TestOutputHelper { get; }

    public TUnitTestClassBase(TUnit.ITestOutputHelper testOutputHelper)
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
