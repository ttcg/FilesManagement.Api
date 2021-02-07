using Serilog.Core;
using Serilog.Events;

namespace FilesManagement.Api.Loggings
{
    public class LogEnricher : ILogEventEnricher
    {
        private string EnvironmentName { get; set; }
        private string MachineName { get; set; }

        public LogEnricher(string environmentName, string machineName)
        {
            EnvironmentName = environmentName;
            MachineName = machineName;
        }

        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            // remove unused properties
            le.RemovePropertyIfPresent("SourceContext");
            le.RemovePropertyIfPresent("RequestId");
            le.RemovePropertyIfPresent("ParentId");
            le.RemovePropertyIfPresent("ActionId");
            le.RemovePropertyIfPresent("ActionName");

            // add new properties
            le.AddPropertyIfAbsent(lepf.CreateProperty("machineName", MachineName));
            le.AddPropertyIfAbsent(lepf.CreateProperty("environment", EnvironmentName));
        }
    }
}
