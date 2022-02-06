using EduTime.ViewModels.Storage;
using DigitalSkynet.DotnetCore.DataStructures.Models.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EduTime.Core.Interfaces.FileStorage
{
    public interface IStorageService
    {
        Task<StorageObjectVm> CreateAsync(IFormFile file, CancellationToken token = default);
        Task<StorageObjectVm> GetAsync(Guid id, CancellationToken token = default);
        Task<StorageObjectVm> UpdateFileAsync(Guid id, IFormFile file, CancellationToken token = default);
        Task DeleteFileAsync(Guid id, CancellationToken token = default);
    }
}
