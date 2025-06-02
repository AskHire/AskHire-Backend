//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/[controller]")]
//public class CandidateFileController : ControllerBase
//{
//    private readonly ICandidateFileService _service;

//    public CandidateFileController(ICandidateFileService service)
//    {
//        _service = service;
//    }

//    [HttpPost("upload")]
//    public async Task<IActionResult> UploadCv([FromQuery] Guid userId, [FromQuery] Guid vacancyId, IFormFile file) =>
//        await _service.UploadCvAsync(userId, vacancyId, file);

//    [HttpGet("download/{applicationId}")]
//    public async Task<IActionResult> DownloadCv(Guid applicationId) =>
//        await _service.DownloadCvAsync(applicationId);

//    [HttpDelete("delete/{applicationId}")]
//    public async Task<IActionResult> DeleteCv(Guid applicationId) =>
//        await _service.DeleteCvAsync(applicationId);

//    [HttpGet("view-uploads")]
//    public async Task<IActionResult> ViewUploadedCvs() =>
//        await _service.ViewUploadedCvsAsync();
//}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CandidateFileController : ControllerBase
{
    private readonly ICandidateFileService _service;

    public CandidateFileController(ICandidateFileService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCv([FromQuery] Guid userId, [FromQuery] Guid vacancyId, IFormFile file)
    {
        return await _service.UploadCvAsync(userId, vacancyId, file);
    }

    [HttpGet("download/{applicationId}")]
    public async Task<IActionResult> DownloadCv(Guid applicationId)
    {
        return await _service.DownloadCvAsync(applicationId);
    }

    [HttpDelete("delete/{applicationId}")]
    public async Task<IActionResult> DeleteCv(Guid applicationId)
    {
        return await _service.DeleteCvAsync(applicationId);
    }

    [HttpGet("view-uploads")]
    public async Task<IActionResult> ViewUploadedCvs()
    {
        return await _service.ViewUploadedCvsAsync();
    }

    [HttpPost("analyze-application/{applicationId}")]
    public async Task<IActionResult> AnalyzeApplication(Guid applicationId)
    {
        return await _service.AnalyzeApplicationAsync(applicationId);
    }
}
