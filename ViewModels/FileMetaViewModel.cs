using System;

namespace FilesManagement.Api.ViewModels
{
    public class FileMetaViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
