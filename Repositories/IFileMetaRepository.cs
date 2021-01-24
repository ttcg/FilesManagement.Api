using FilesManagement.Api.Aggregates;
using System;
using System.Threading.Tasks;

namespace FilesManagement.Api.Repositories
{
    public interface IFileMetaRepository
    {
        public Task<FileMeta> GetById(Guid id);
        public Task<FileMeta> Add(FileMeta fileMeta);
    }
}
