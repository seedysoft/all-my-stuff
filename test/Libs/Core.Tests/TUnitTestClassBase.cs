namespace Seedysoft.Libs.Core.Tests;

public abstract class TUnitTestClassBase
{
    public TUnitTestClassBase()
    {
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
