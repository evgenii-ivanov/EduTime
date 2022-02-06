using AutoMapper;
using DigitalSkynet.Boilerplate.Data.Interfaces.FileStorage;
using EduTime.Domain.Entities.FileStorage;
using EduTime.ViewModels.Storage;
using DigitalSkynet.DotnetCore.DataAccess.UnitOfWork;
using DigitalSkynet.DotnetCore.DataStructures.Exceptions.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DigitalSkynet.DotnetCore.DataAccess.Enums;
using EduTime.Core.Interfaces.FileStorage;
using EduTime.Foundation.Options;

namespace EduTime.Core.Services.FileStorage
{
    public class LocalStorageService : IStorageService
    {
        private readonly FileStorageOptions _fileStorageOptions;
        private readonly IStorageObjectRepository _storageObjectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocalStorageService(IOptions<FileStorageOptions> fileStorageOptions, IStorageObjectRepository storageObjectRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _fileStorageOptions = fileStorageOptions.Value;
            _storageObjectRepository = storageObjectRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StorageObjectVm> CreateAsync(IFormFile file, CancellationToken ct = default)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid() + extension;
            var path = GetPath(fileName);

            await SaveFileAsync(path, file, ct);
            var storage = new StorageObject
            {
                Name = fileName,
                MimeType = file.ContentType,
                Extension = extension,
                Path = fileName
            };

            _storageObjectRepository.Create(storage);
            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<StorageObjectVm>(storage);
        }

        public async Task<StorageObjectVm> GetAsync(Guid id, CancellationToken ct = default)
        {
            var file = await GetFileFromDbAsync(id, ct);
            return _mapper.Map<StorageObjectVm>(file);
        }

        public async Task DeleteFileAsync(Guid id, CancellationToken ct = default)
        {
            var file = await GetFileFromDbAsync(id, ct);
            var path = GetPath(file.Path);

            File.Delete(path);
            await _storageObjectRepository.DeleteHardAsync(id, ct: ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<StorageObjectVm> UpdateFileAsync(Guid id, IFormFile file, CancellationToken ct = default)
        {
            var dbFile = await GetFileFromDbAsync(id, ct);
            var path = GetPath(dbFile.Path);

            File.Delete(path);
            await SaveFileAsync(path, file, ct);

            dbFile.UpdatedDate = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);

            return _mapper.Map<StorageObjectVm>(dbFile);
        }

        private async Task<StorageObject> GetFileFromDbAsync(Guid id, CancellationToken ct = default)
        {
            var dbFile = await _storageObjectRepository.GetByIdAsync(id, FetchModes.Tracking, ct);
            if (dbFile == null)
            {
                throw new ApiNotFoundException("File not found");
            }

            var path = GetPath(dbFile.Path);
            if (!File.Exists(path))
            {
                throw new ApiNotFoundException("File not found");
            }
            return dbFile;
        }

        private string GetPath(string storedPath) => Path.Combine(_fileStorageOptions.LocalPath, storedPath);

        private static async Task SaveFileAsync(string path, IFormFile file, CancellationToken ct = default)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            var stream = memoryStream.ToArray();
            await File.WriteAllBytesAsync(path, stream, ct);
        }
    }
}
