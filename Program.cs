using FilesManagement.Api.Loggings;
using FilesManagement.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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

            var seqSettings =  config.GetSection("SeqSettings").Get<SeqSettings>();

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
                    webBuilder.UseStartup<Startup>();
                });

        public static Logger CreateLogger(AppConfig.SeqConfig seqConfig)
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.With(new LogEnricher())
                .WriteTo.Seq(seqConfig.ServerUrl, apiKey: seqConfig.ApiKey)
                .WriteTo.Console()
                .CreateLogger();

            var loggerFactory = (ILoggerFactory)new LoggerFactory();
            loggerFactory.AddSerilog(serilogLogger);

            return serilogLogger;
        }
    }
}
