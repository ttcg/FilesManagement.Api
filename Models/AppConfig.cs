using FilesManagement.Api.Options;
using Microsoft.Extensions.Options;

namespace FilesManagement.Api.Models
{
    public class AppConfig
    {
        public SeqConfig SeqSettings { get; set; }
        public GcpStorageConfig GcpStorageSettings { get; set; }

        public AppConfig(IOptions<GcpStorageOption> gcpStorageOption)
        {
            GcpStorageSettings = new GcpStorageConfig
            {
                JsonCredential = gcpStorageOption.Value.JsonCredential,
                BucketName = gcpStorageOption.Value.BucketName
            };
        }

        public class GcpStorageConfig
        {
            public string BucketName { get; set; }
            public string JsonCredential { get; set; }
        }

        public class SeqConfig
        {
            public string ApiKey { get; set; }
            public string ServerUrl { get; set; }
        }
    }
}
