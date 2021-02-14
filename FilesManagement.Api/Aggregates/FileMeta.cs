using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilesManagement.Api.Aggregates
{
    public class FileMeta
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BucketName { get; set; }
        public string Link { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
