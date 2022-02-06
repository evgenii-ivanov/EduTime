using EduTime.Core.Interfaces.FileStorage;
using DigitalSkynet.DotnetCore.DataStructures.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using EduTime.ViewModels.Storage;

namespace EduTime.Api.Controllers
{
    public class StorageController : AppBaseController
    {
        private readonly IStorageService _fileService;

        public StorageController(IStorageService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponseEnvelope<StorageObjectVm>>> Get(Guid id, CancellationToken ct = default)
        {
            var result = await _fileService.GetAsync(id, ct);
            return ResponseModel(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseEnvelope<StorageObjectVm>>> Upload([FromForm] IFormFile file, CancellationToken ct = default)
        {
            var result = await _fileService.CreateAsync(file, ct);
            return ResponseModel(result);
        }

        [HttpPost("{id:guid}")]
        public async Task<ActionResult<ApiResponseEnvelope<StorageObjectVm>>> Update(Guid id, [FromForm] IFormFile file, CancellationToken ct = default)
        {
            var result = await _fileService.UpdateFileAsync(id, file, ct);
            return ResponseModel(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponseEnvelope<object>>> Delete(Guid id, CancellationToken ct = default)
        {
            await _fileService.DeleteFileAsync(id, ct);
            return ResponseModel<object>(null);
        }

        // TODO: get public link method
    }
}
