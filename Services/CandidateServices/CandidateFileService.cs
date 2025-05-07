using AskHire_Backend.Models.Entities;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mime;

public class CandidateFileService : ICandidateFileService
{
    private readonly ICandidateFileRepository _repository;
    private readonly BlobContainerClient _blobContainer;

    public CandidateFileService(ICandidateFileRepository repository, IConfiguration config)
    {
        _repository = repository;

        string connectionString = config["AzureBlobStorage:ConnectionString"]!;
        string containerName = config["AzureBlobStorage:ContainerName"]!;
        _blobContainer = new BlobContainerClient(connectionString, containerName);
        _blobContainer.CreateIfNotExists();
    }

    public async Task<IActionResult> UploadCvAsync(Guid userId, Guid vacancyId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return new BadRequestObjectResult("Invalid file.");

        var user = await _repository.GetUserAsync(userId);
        var vacancy = await _repository.GetVacancyAsync(vacancyId);
        if (user == null || vacancy == null)
            return new BadRequestObjectResult("Invalid user or vacancy ID.");

        var applicationId = Guid.NewGuid();
        var blobName = $"{applicationId}/{file.FileName}";
        var blobClient = _blobContainer.GetBlobClient(blobName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        var application = new Application
        {
            ApplicationId = applicationId,
            UserId = userId,
            VacancyId = vacancyId,
            User = user,
            Vacancy = vacancy,
            CV_Mark = 0,
            Pre_Screen_PassMark = 0,
            Status = "Pending",
            DashboardStatus = "CV Uploaded",
            CVFilePath = $"{_blobContainer.Uri}/{blobName}"
        };

        await _repository.AddApplicationAsync(application);
        await _repository.SaveChangesAsync();

        return new OkObjectResult(new { ApplicationId = applicationId, Message = "CV uploaded successfully." });
    }

    public async Task<IActionResult> DownloadCvAsync(Guid applicationId)
    {
        var application = await _repository.GetApplicationAsync(applicationId);
        if (application == null || string.IsNullOrEmpty(application.CVFilePath))
            return new NotFoundObjectResult("CV not found.");

        var blobUrl = new Uri(application.CVFilePath);
        var blobName = blobUrl.AbsolutePath.TrimStart('/').Replace($"{_blobContainer.Name}/", "");
        var blob = _blobContainer.GetBlobClient(blobName);

        if (!await blob.ExistsAsync())
            return new NotFoundObjectResult("Blob does not exist.");

        var downloadInfo = await blob.DownloadAsync();
        return new FileStreamResult(downloadInfo.Value.Content, MediaTypeNames.Application.Octet)
        {
            FileDownloadName = Path.GetFileName(blobName)
        };
    }

    public async Task<IActionResult> DeleteCvAsync(Guid applicationId)
    {
        var application = await _repository.GetApplicationAsync(applicationId);
        if (application == null || string.IsNullOrEmpty(application.CVFilePath))
            return new NotFoundObjectResult("CV not found.");

        var blobUrl = new Uri(application.CVFilePath);
        var blobName = blobUrl.AbsolutePath.TrimStart('/').Replace($"{_blobContainer.Name}/", "");
        var blob = _blobContainer.GetBlobClient(blobName);

        await blob.DeleteIfExistsAsync();

        application.CVFilePath = null!;
        await _repository.SaveChangesAsync();

        return new OkObjectResult("CV deleted successfully.");
    }

    public async Task<IActionResult> ViewUploadedCvsAsync()
    {
        var result = new List<object>();

        await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
        {
            result.Add(new
            {
                Name = blobItem.Name,
                Url = $"{_blobContainer.Uri}/{blobItem.Name}",
                Size = blobItem.Properties.ContentLength,
                ContentType = blobItem.Properties.ContentType,
                LastModified = blobItem.Properties.LastModified?.UtcDateTime
            });
        }

        return new OkObjectResult(result);
    }
}
