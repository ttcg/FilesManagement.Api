using Serilog.Core;
using Serilog.Events;
using System;

namespace FilesManagement.Api.Loggings
{
    public class LogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            // remove unused properties
            le.RemovePropertyIfPresent("SourceContext");
            le.RemovePropertyIfPresent("RequestId");
            le.RemovePropertyIfPresent("ActionId");
            le.RemovePropertyIfPresent("ActionName");

            // add new properties
            le.AddPropertyIfAbsent(lepf.CreateProperty("MachineName", Environment.MachineName));
            le.AddPropertyIfAbsent(lepf.CreateProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")));
        }
    }
}
