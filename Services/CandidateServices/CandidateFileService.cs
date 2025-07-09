//using AskHire_Backend.Models.Entities;
//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System.Net.Mime;
//using System.Text;
//using UglyToad.PdfPig;


//public class CandidateFileService : ICandidateFileService
//{
//    private readonly ICandidateFileRepository _repository;
//    private readonly BlobContainerClient _blobContainer;

//    public CandidateFileService(ICandidateFileRepository repository, IConfiguration config)
//    {
//        _repository = repository;

//        string connectionString = config["AzureBlobStorage:ConnectionString"]!;
//        string containerName = config["AzureBlobStorage:ContainerName"]!;
//        _blobContainer = new BlobContainerClient(connectionString, containerName);
//        _blobContainer.CreateIfNotExists();
//    }

//    public async Task<IActionResult> UploadCvAsync(Guid userId, Guid vacancyId, IFormFile file)
//    {
//        if (file == null || file.Length == 0)
//            return new BadRequestObjectResult("Invalid file.");

//        var user = await _repository.GetUserAsync(userId);
//        var vacancy = await _repository.GetVacancyAsync(vacancyId);
//        if (user == null || vacancy == null)
//            return new BadRequestObjectResult("Invalid user or vacancy ID.");

//        var applicationId = Guid.NewGuid();
//        var blobName = $"{applicationId}/{file.FileName}";
//        var blobClient = _blobContainer.GetBlobClient(blobName);

//        // Upload the file to Azure Blob Storage
//        using (var stream = file.OpenReadStream())
//        {
//            await blobClient.UploadAsync(stream, overwrite: true);
//        }

//        // Extract text from the uploaded PDF using PdfPig
//        string extractedText = string.Empty;
//        if (file.ContentType == "application/pdf")
//        {
//            using var pdfStream = file.OpenReadStream();
//            using var pdfDocument = PdfDocument.Open(pdfStream);
//            var textBuilder = new StringBuilder();

//            foreach (var page in pdfDocument.GetPages())
//            {
//                textBuilder.AppendLine(page.Text);
//            }

//            extractedText = textBuilder.ToString();
//        }

//        // Create Application record
//        var application = new Application
//        {
//            ApplicationId = applicationId,
//            UserId = userId,
//            User = user,
//            VacancyId = vacancyId,
//            Vacancy = vacancy,
//            CVFilePath = blobClient.Uri.ToString(),
//            CV_Mark = 0,
//            Pre_Screen_PassMark = 0,
//            Status = "Pending",
//            DashboardStatus = "Awaiting Review"
//        };

//        await _repository.AddApplicationAsync(application);
//        await _repository.SaveChangesAsync();

//        return new OkObjectResult(new
//        {
//            ApplicationId = applicationId,
//            Message = "CV uploaded and application saved successfully.",
//            ExtractedCvText = extractedText,
//            BlobUrl = blobClient.Uri.ToString()
//        });
//    }


//    public async Task<IActionResult> DownloadCvAsync(Guid applicationId)
//    {
//        var application = await _repository.GetApplicationAsync(applicationId);
//        if (application == null || string.IsNullOrEmpty(application.CVFilePath))
//            return new NotFoundObjectResult("CV not found.");

//        var blobUrl = new Uri(application.CVFilePath);
//        var blobName = blobUrl.AbsolutePath.TrimStart('/').Replace($"{_blobContainer.Name}/", "");
//        var blob = _blobContainer.GetBlobClient(blobName);

//        if (!await blob.ExistsAsync())
//            return new NotFoundObjectResult("Blob does not exist.");

//        var downloadInfo = await blob.DownloadAsync();
//        return new FileStreamResult(downloadInfo.Value.Content, MediaTypeNames.Application.Octet)
//        {
//            FileDownloadName = Path.GetFileName(blobName)
//        };
//    }

//    public async Task<IActionResult> DeleteCvAsync(Guid applicationId)
//    {
//        var application = await _repository.GetApplicationAsync(applicationId);
//        if (application == null || string.IsNullOrEmpty(application.CVFilePath))
//            return new NotFoundObjectResult("CV not found.");

//        var blobUrl = new Uri(application.CVFilePath);
//        var blobName = blobUrl.AbsolutePath.TrimStart('/').Replace($"{_blobContainer.Name}/", "");
//        var blob = _blobContainer.GetBlobClient(blobName);

//        await blob.DeleteIfExistsAsync();

//        application.CVFilePath = null!;
//        await _repository.SaveChangesAsync();

//        return new OkObjectResult("CV deleted successfully.");
//    }

//    public async Task<IActionResult> ViewUploadedCvsAsync()
//    {
//        var result = new List<object>();

//        await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
//        {
//            result.Add(new
//            {
//                Name = blobItem.Name,
//                Url = $"{_blobContainer.Uri}/{blobItem.Name}",
//                Size = blobItem.Properties.ContentLength,
//                ContentType = blobItem.Properties.ContentType,
//                LastModified = blobItem.Properties.LastModified?.UtcDateTime
//            });
//        }

//        return new OkObjectResult(result);
//    }
//}

using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.Entities;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UglyToad.PdfPig;

public class CandidateFileService : ICandidateFileService
{
    private readonly ICandidateFileRepository _repository;
    private readonly BlobContainerClient _blobContainer;
    private readonly GeminiHelper _geminiHelper;

    public CandidateFileService(ICandidateFileRepository repository, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;

        string connectionString = config["AzureBlobStorage:ConnectionString"]!;
        string containerName = config["AzureBlobStorage:ContainerName"]!;
        _blobContainer = new BlobContainerClient(connectionString, containerName);
        _blobContainer.CreateIfNotExists();

        string geminiApiKey = config["GeminiApiKey"]!;
        _geminiHelper = new GeminiHelper(httpClientFactory, geminiApiKey);
    }

    //public async Task<IActionResult> UploadCvAsync(Guid userId, Guid vacancyId, IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        return new BadRequestObjectResult("Invalid file.");

    //    var user = await _repository.GetUserAsync(userId);
    //    var vacancy = await _repository.GetVacancyAsync(vacancyId);
    //    if (user == null || vacancy == null)
    //        return new BadRequestObjectResult("Invalid user or vacancy ID.");

    //    var applicationId = Guid.NewGuid();
    //    var blobName = $"{applicationId}/{file.FileName}";
    //    var blobClient = _blobContainer.GetBlobClient(blobName);

    //    using (var stream = file.OpenReadStream())
    //    {
    //        await blobClient.UploadAsync(stream, overwrite: true);
    //    }

    //    string extractedText = string.Empty;
    //    if (file.ContentType == "application/pdf")
    //    {
    //        using var pdfStream = file.OpenReadStream();
    //        using var pdfDocument = PdfDocument.Open(pdfStream);
    //        var textBuilder = new StringBuilder();

    //        foreach (var page in pdfDocument.GetPages())
    //        {
    //            textBuilder.AppendLine(page.Text);
    //        }

    //        extractedText = textBuilder.ToString();
    //    }

    //    var application = new Application
    //    {
    //        ApplicationId = applicationId,
    //        UserId = userId,
    //        User = user,
    //        VacancyId = vacancyId,
    //        Vacancy = vacancy,
    //        CVFilePath = blobClient.Uri.ToString(),
    //        CV_Mark = 0,
    //        Pre_Screen_PassMark = 0,
    //        Status = "Pending",
    //        DashboardStatus = "Awaiting Review"
    //    };

    //    await _repository.AddApplicationAsync(application);
    //    await _repository.SaveChangesAsync();

    //    return new OkObjectResult(new
    //    {
    //        ApplicationId = applicationId,
    //        Message = "CV uploaded and application saved successfully.",
    //        ExtractedCvText = extractedText,
    //        BlobUrl = blobClient.Uri.ToString()
    //    });
    //}

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

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        string extractedText = string.Empty;
        if (file.ContentType == "application/pdf")
        {
            using var pdfStream = file.OpenReadStream();
            using var pdfDocument = PdfDocument.Open(pdfStream);
            var textBuilder = new StringBuilder();

            foreach (var page in pdfDocument.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }

            extractedText = textBuilder.ToString();
        }

        var application = new Application
        {
            ApplicationId = applicationId,
            UserId = userId,
            User = user,
            VacancyId = vacancyId,
            Vacancy = vacancy,
            CVFilePath = blobClient.Uri.ToString(),
            CV_Mark = 0,
            Pre_Screen_PassMark = 0,
            Status = "Pending",
            DashboardStatus = "Applied"
        };

        await _repository.AddApplicationAsync(application);
        await _repository.SaveChangesAsync();

        // 🧠 Call the AnalyzeApplicationAsync function here
        var analysisResult = await AnalyzeApplicationAsync(applicationId);
        if (analysisResult is ObjectResult result)
        {
            return new OkObjectResult(new
            {
                ApplicationId = applicationId,
                Message = "CV uploaded, application saved, and analysis completed successfully.",
                ExtractedCvText = extractedText,
                BlobUrl = blobClient.Uri.ToString(),
                AnalysisResult = result.Value
            });
        }

        return new OkObjectResult(new
        {
            ApplicationId = applicationId,
            Message = "CV uploaded and application saved, but analysis did not return data.",
            ExtractedCvText = extractedText,
            BlobUrl = blobClient.Uri.ToString()
        });
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

    //public async Task<IActionResult> AnalyzeApplicationAsync(Guid applicationId)
    //{
    //    var application = await _repository.GetApplicationWithVacancyAsync(applicationId);
    //    if (application == null)
    //        return new NotFoundObjectResult($"Application with ID {applicationId} not found.");

    //    var vacancy = application.Vacancy;
    //    if (vacancy == null)
    //        return new BadRequestObjectResult("Vacancy information not available for this application.");

    //    var requiredSkills = vacancy.RequiredSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new();
    //    var nonTechnicalSkills = vacancy.NonTechnicalSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new();
    //    var allRequiredSkills = requiredSkills.Concat(nonTechnicalSkills).ToList();

    //    string extractedText = string.Empty;
    //    if (!string.IsNullOrEmpty(application.CVFilePath))
    //    {
    //        var blobUrl = new Uri(application.CVFilePath);
    //        var blobName = blobUrl.AbsolutePath.TrimStart('/').Replace($"{_blobContainer.Name}/", "");
    //        var blob = _blobContainer.GetBlobClient(blobName);

    //        if (await blob.ExistsAsync())
    //        {
    //            var downloadInfo = await blob.DownloadAsync();
    //            using var memoryStream = new MemoryStream();
    //            await downloadInfo.Value.Content.CopyToAsync(memoryStream);
    //            memoryStream.Position = 0;

    //            using var pdfDocument = PdfDocument.Open(memoryStream);

    //            var textBuilder = new StringBuilder();

    //            foreach (var page in pdfDocument.GetPages())
    //            {
    //                textBuilder.AppendLine(page.Text);
    //            }

    //            extractedText = textBuilder.ToString();
    //        }
    //    }

    //    var analyzeRequest = new CvAnalysisRequest
    //    {
    //        CvText = extractedText,
    //        JobTitle = vacancy.VacancyName,
    //        RequiredEducation = vacancy.Education,
    //        RelatedEducation = await _geminiHelper.GetRelatedDegreesAsync(vacancy.Education),
    //        RequiredExperience = vacancy.Experience,
    //        RequiredSkills = allRequiredSkills
    //    };

    //    var analysisResult = await _geminiHelper.AnalyzeCvAsync(analyzeRequest, requiredSkills, nonTechnicalSkills);

    //    application.CV_Mark = (int)Math.Round(analysisResult.OverallScore);

    //    if (analysisResult.OverallScore >= vacancy.CVPassMark)
    //    {
    //        application.Status = "CV Approved";
    //        application.DashboardStatus = "Awaiting Pre-Screen";
    //    }
    //    else
    //    {
    //        application.Status = "CV Rejected";
    //        application.DashboardStatus = "Application Closed";
    //    }

    //    await _repository.SaveChangesAsync();

    //    return new OkObjectResult(new CvAnalysisResponseDto
    //    {
    //        ApplicationId = application.ApplicationId,
    //        CvMark = application.CV_Mark,
    //        Status = application.Status,
    //        DashboardStatus = application.DashboardStatus,
    //        AnalysisDetails = analysisResult
    //    });
    //}

    public async Task<IActionResult> AnalyzeApplicationAsync(Guid applicationId)
    {
        var application = await _repository.GetApplicationWithVacancyAsync(applicationId);
        if (application == null)
            return new NotFoundObjectResult($"Application with ID {applicationId} not found.");

        var vacancy = application.Vacancy;
        if (vacancy == null)
            return new BadRequestObjectResult("Vacancy information not available for this application.");

        if (string.IsNullOrEmpty(application.CVFilePath))
        {
            return new BadRequestObjectResult("No CV file path found for this application.");
        }

        try
        {
            var blobUrl = new Uri(application.CVFilePath);
            var blobName = ExtractBlobNameFromUrl(blobUrl);
            var blob = _blobContainer.GetBlobClient(blobName);

            var exists = await blob.ExistsAsync();
            if (!exists.Value)
            {
                var foundBlobName = await FindBlobByPattern(applicationId.ToString());
                if (foundBlobName != null)
                {
                    blob = _blobContainer.GetBlobClient(foundBlobName);
                }
                else
                {
                    return new BadRequestObjectResult($"CV file not found in blob storage. Blob name: '{blobName}'");
                }
            }

            // Extract text from PDF
            var downloadInfo = await blob.DownloadAsync();
            using var memoryStream = new MemoryStream();
            await downloadInfo.Value.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var pdfDocument = PdfDocument.Open(memoryStream);
            var textBuilder = new StringBuilder();

            foreach (var page in pdfDocument.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }

            var extractedText = textBuilder.ToString();

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                return new BadRequestObjectResult("Failed to extract text from CV file or CV is empty.");
            }

            // Continue with analysis
            var requiredSkills = vacancy.RequiredSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var nonTechnicalSkills = vacancy.NonTechnicalSkills?.Split(',').Select(s => s.Trim()).ToList() ?? new();
            var allRequiredSkills = requiredSkills.Concat(nonTechnicalSkills).ToList();

            var analyzeRequest = new CvAnalysisRequest
            {
                CvText = extractedText,
                JobTitle = vacancy.VacancyName,
                RequiredEducation = vacancy.Education,
                RelatedEducation = await _geminiHelper.GetRelatedDegreesAsync(vacancy.Education),
                RequiredExperience = vacancy.Experience,
                RequiredSkills = allRequiredSkills
            };

            var analysisResult = await _geminiHelper.AnalyzeCvAsync(analyzeRequest, requiredSkills, nonTechnicalSkills);

            // Handle missing OverallScore - calculate it if not provided
            if (analysisResult.OverallScore == 0)
            {
                analysisResult.OverallScore = CalculateOverallScore(analysisResult);
            }

            application.CV_Mark = (int)Math.Round(analysisResult.OverallScore);

            if (analysisResult.OverallScore >= vacancy.CVPassMark)
            {
                application.Status = "CV Approved";
                application.DashboardStatus = "Awaiting Pre-Screen";
            }
            else
            {
                application.Status = "CV Rejected";
                application.DashboardStatus = "Application Closed";
            }

            await _repository.SaveChangesAsync();

            return new OkObjectResult(new CvAnalysisResponseDto
            {
                ApplicationId = application.ApplicationId,
                CvMark = application.CV_Mark,
                Status = application.Status,
                DashboardStatus = application.DashboardStatus,
                AnalysisDetails = analysisResult
            });
        }
        catch (JsonException jsonEx)
        {
            return new BadRequestObjectResult($"Failed to parse Gemini response: {jsonEx.Message}");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult($"Error processing CV: {ex.Message}");
        }
    }

    private double CalculateOverallScore(dynamic analysisResult)
    {
        try
        {
            // Get scores from different sections
            double educationScore = 0;
            double experienceScore = 0;
            double skillsScore = 0;

            // Try to extract scores from the analysis result
            if (analysisResult.EducationAnalysis?.DegreeRelevanceScore != null)
                educationScore = (double)analysisResult.EducationAnalysis.DegreeRelevanceScore;

            if (analysisResult.ExperienceAnalysis?.ExperienceScore != null)
                experienceScore = (double)analysisResult.ExperienceAnalysis.ExperienceScore;

            if (analysisResult.SkillsAnalysis?.SkillsScore != null)
                skillsScore = (double)analysisResult.SkillsAnalysis.SkillsScore;

            // Calculate weighted average (you can adjust weights as needed)
            double educationWeight = 0.3;
            double experienceWeight = 0.4;
            double skillsWeight = 0.3;

            double overallScore = (educationScore * educationWeight) +
                                 (experienceScore * experienceWeight) +
                                 (skillsScore * skillsWeight);

            return Math.Round(overallScore, 2);
        }
        catch
        {
            // Fallback to a default score or throw an exception
            return 0;
        }
    }

    private string ExtractBlobNameFromUrl(Uri blobUrl)
    {
        var absolutePath = blobUrl.AbsolutePath.TrimStart('/');
        var decodedPath = Uri.UnescapeDataString(absolutePath);

        if (decodedPath.StartsWith($"{_blobContainer.Name}/"))
        {
            decodedPath = decodedPath.Substring($"{_blobContainer.Name}/".Length);
        }

        return decodedPath;
    }

    private async Task<string?> FindBlobByPattern(string applicationId)
    {
        try
        {
            await foreach (var blobItem in _blobContainer.GetBlobsAsync())
            {
                if (blobItem.Name.StartsWith(applicationId))
                {
                    return blobItem.Name;
                }
            }
        }
        catch (Exception)
        {
            // If listing fails, return null
        }

        return null;
    }
    //public async Task<int?> GetCVMarkAsync(Guid applicationId)
    //{
    //    return await _repository.GetCVMarkByApplicationIdAsync(applicationId);
    //}

    public Task<ApplicationCVStatusDto?> GetCVMarkAndStatusAsync(Guid applicationId)
    {
        return _repository.GetCVMarkAndStatusAsync(applicationId);
    }

}

