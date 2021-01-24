using FilesManagement.Api.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManagement.Api.Repositories
{
    public class FileMetaRepository : IFileMetaRepository
    {
        private List<FileMeta> _records;

        public FileMetaRepository()
        {
            _records = new List<FileMeta>
            {
                new FileMeta
                {
                    Id= Guid.Parse( "76d4f9a5-3780-4a45-9004-75e3980f9a42"),
                    Name = "76d4f9a5-3780-4a45-9004-75e3980f9a42/Test File.txt"
                },
                new FileMeta
                {
                    Id= Guid.Parse( "9c406c21-0c7b-4caa-8ae2-27fb839d812e"),
                    Name = "9c406c21-0c7b-4caa-8ae2-27fb839d812e/6321.jpg"
                }
            };
        }
        public async Task<FileMeta> Add(FileMeta fileMeta)
        {
            return await Task.Run(() =>
            {
                fileMeta.DateCreated = DateTime.UtcNow;
                fileMeta.DateModified = DateTime.UtcNow;

                _records.Add(fileMeta);

                return fileMeta;
            });
        }

        public async Task<FileMeta> GetById(Guid id)
        {
            return await Task.Run(() =>
            {
                return _records.SingleOrDefault(c => c.Id == id);
            });
        }
    }

}
