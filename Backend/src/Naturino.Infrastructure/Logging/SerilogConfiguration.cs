using Microsoft.Extensions.Configuration;
using Serilog;

namespace Naturino.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static LoggerConfiguration ConfigureSerilog(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
    {
        return loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/naturino-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14);
    }
}
