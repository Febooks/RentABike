using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RentABike.Domain.Interfaces;

namespace RentABike.Infrastructure.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public LocalStorageService(IConfiguration configuration, IHostEnvironment environment)
    {
        var relativePath = configuration["Storage:Local:BasePath"] ?? "wwwroot/uploads";
        _basePath = Path.IsPathRooted(relativePath) 
            ? relativePath 
            : Path.Combine(environment.ContentRootPath, relativePath);
        
        _baseUrl = configuration["Storage:Local:BaseUrl"] ?? "/uploads";
        
        var directory = Path.GetDirectoryName(_basePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_basePath, uniqueFileName);

        using (var fileStreamWriter = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamWriter);
        }

        return $"{_baseUrl}/{uniqueFileName}";
    }

    public Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            var fileName = Path.GetFileName(fileUrl);
            var filePath = Path.Combine(_basePath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}

