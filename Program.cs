using FilesManagement.Api.Loggings;
using FilesManagement.Api.Models;
using FilesManagement.Api.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace FilesManagement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var seqSettings = config.GetSection("SeqSettings").Get<SeqSettings>();

            Log.Logger = CreateLogger(new AppConfig.SeqConfig
            {
                ApiKey = seqSettings.ApiKey,
                ServerUrl = seqSettings.ServerUrl
            });

            try
            {
                Log.Information("Starting up Files Management Api");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Files Management Api terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .ConfigureServices(services =>
                    {
                        var config = new ConfigurationBuilder()
                            .AddUserSecrets<Program>()
                            .Build();

                        services.Configure<GcpStorageOption>(config.GetSection("GcpStorageSettings"));
                        services.AddSingleton<AppConfig>();
                    })
                    .ConfigureKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = 21_457_280;
                    })                    
                    .UseStartup<Startup>();
                });

        public static Logger CreateLogger(AppConfig.SeqConfig seqConfig)
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.With(new LogEnricher(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), Environment.MachineName))
                .WriteTo.Seq(seqConfig.ServerUrl, apiKey: seqConfig.ApiKey)
                .WriteTo.Console()
                .CreateLogger();

            var loggerFactory = (ILoggerFactory)new LoggerFactory();
            loggerFactory.AddSerilog(serilogLogger);

            return serilogLogger;
        }
    }
}
