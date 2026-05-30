using System;
using System.Runtime.CompilerServices;

namespace TesteApp;

public static class TestInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }
}
