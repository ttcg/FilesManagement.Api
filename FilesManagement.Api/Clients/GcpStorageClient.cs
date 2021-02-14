using FilesManagement.Api.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FilesManagement.Api.Clients
{
    public class GcpStorageClient : IStorageClient
    {
        private readonly StorageClient _storageClient;
        private readonly ILogger<GcpStorageClient> _logger;
        private readonly AppConfig.GcpStorageConfig _gcpStorageConfig;

        public GcpStorageClient(ILogger<GcpStorageClient> logger, AppConfig appConfig)
        {
            _gcpStorageConfig = appConfig.GcpStorageSettings;

            _storageClient = StorageClient.Create(GoogleCredential.FromJson(_gcpStorageConfig.JsonCredential));
            _logger = logger;
        }

        public async Task<FileMeta> UploadAsync(Guid fileId, string fileName, string contentType, Stream stream)
        {
            // using fileId/filename to store a file in unique folders
            //var file = await _storageClient.UploadObjectAsync(_gcpStorageConfig.BucketName, $"{fileId}/{fileName}", null, stream);

            // using fileId to store it as a file name on the flat level
            var file = await _storageClient.UploadObjectAsync(_gcpStorageConfig.BucketName, fileName, contentType, stream);

            var fileMeta = new FileMeta
            {
                Id = file.Id,
                Name = file.Name,
                Link = file.SelfLink
            };

            _logger.LogInformation("uploaded to {bucket}|{@message}", _gcpStorageConfig.BucketName, fileMeta);

            return fileMeta;
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            var stream = new MemoryStream();

            await _storageClient.DownloadObjectAsync(_gcpStorageConfig.BucketName, fileName, stream);

            return stream;
        }
    }
}
