using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public interface ICandidateFileService
{
    Task<IActionResult> UploadCvAsync(Guid userId, Guid vacancyId, IFormFile file);
    Task<IActionResult> DownloadCvAsync(Guid applicationId);
    Task<IActionResult> DeleteCvAsync(Guid applicationId);
    Task<IActionResult> ViewUploadedCvsAsync();
}
