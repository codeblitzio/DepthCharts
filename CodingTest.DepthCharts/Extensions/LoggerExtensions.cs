using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;

namespace CodingTest.DepthCharts.Extensions
{
    public static class LoggerExtensions
    {
        public static LoggerConfiguration Configure(this LoggerConfiguration config)
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.Console(new JsonFormatter());

            return config;
        }
    }
}