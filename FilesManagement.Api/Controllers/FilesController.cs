using FilesManagement.Api.Clients;
using FilesManagement.Api.Repositories;
using FilesManagement.Api.ViewModels;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeTypeMap.List;
using FilesManagement.Api.Security;

namespace FilesManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IStorageClient _storageClient;
        private readonly IFileMetaRepository _fileMetaRepository;

        string CurrentActionName => ControllerContext.RouteData.Values["action"].ToString();

        public FilesController(ILogger<FilesController> logger, IStorageClient storageClient, IFileMetaRepository fileMetaRepository)
        {
            _logger = logger;
            _storageClient = storageClient;
            _fileMetaRepository = fileMetaRepository;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get file metadata",
            Description = "Get file metadata by Id"
        )]
        [AllowedUser("11")]
        public async Task<ActionResult<FileMetaViewModel>> GetById(Guid id)
        {
            var fileMeta = await _fileMetaRepository.GetById(id);

            if (fileMeta == null)
            {
                return NotFound($"FileId: {id} does not exist");
            }

            var viewModel = new FileMetaViewModel
            {
                Id = id,
                Name = fileMeta.Name,
                Link = fileMeta.Link,
                DateCreated = fileMeta.DateCreated,
                DateModified = fileMeta.DateModified
            };

            _logger.LogInformation("{messageName}|{@message}", CurrentActionName, viewModel);

            return viewModel;
        }

        [HttpPost("Upload")]
        [SwaggerOperation(
            Summary = "Upload file",
            Description = "Upload a file to file storage"
        )]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            var fileId = Guid.NewGuid();
            var fileExt = Path.GetExtension(formFile.FileName);
            var fileName = $"{fileId}{fileExt}";
            var contentType = MimeTypeMap.List.MimeTypeMap.GetMimeType(fileExt).First(); // the library will always return "application/octet-stream" if type not found


            var fileMetaModel = await _storageClient.UploadAsync(fileId, fileName, contentType, formFile.OpenReadStream());

            // save uploaded file metadata in persisted store for future reference
            await _fileMetaRepository.Add(new Aggregates.FileMeta
            {
                Id = fileId,
                Link = fileMetaModel.Link,
                Name = fileMetaModel.Name
            });

            var viewModel = new FileUploadViewModel
            {
                Id = fileId.ToString()
            };

            _logger.LogInformation("{messageName}|{@message}", CurrentActionName, viewModel);

            return new ObjectResult(viewModel);
        }

        [HttpGet("Download")]
        [SwaggerOperation(
            Summary = "Download file",
            Description = "Download a file from file storage"
        )]
        public async Task<IActionResult> Download(Guid id, string name = null)
        {
            var fileMeta = await _fileMetaRepository.GetById(id);

            if (fileMeta == null)
            {
                return NotFound($"FileId: {id} does not exist");
            }

            _logger.LogInformation("{messageName}|{@message}", CurrentActionName, fileMeta);

            var stream = await _storageClient.DownloadAsync(fileMeta.Name);

            stream.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(stream, System.Net.Mime.MediaTypeNames.Application.Octet)
            {
                FileDownloadName = GetFileDownloadName()
            };

            string GetFileDownloadName() => string.IsNullOrWhiteSpace(name) ? fileMeta.Name : name;
        }
    }
}
