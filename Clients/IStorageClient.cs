using FilesManagement.Api.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FilesManagement.Api.Clients
{
    public interface IStorageClient
    {
        public Task<FileMeta> UploadAsync(Guid fileId, string fileName, Stream stream);
        public Task<Stream> DownloadAsync(string fileName);
    }
}
