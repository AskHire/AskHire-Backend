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

    //[HttpGet("{applicationId}/cvmark")]
    //public async Task<IActionResult> GetCVMark(Guid applicationId)
    //{
    //    var cvMark = await _service.GetCVMarkAsync(applicationId);

    //    if (cvMark == null)
    //    {
    //        return NotFound("Application not found");
    //    }

    //    return Ok(cvMark);
    //}

    //[HttpGet("{applicationId}/cv-status")]
    //public async Task<IActionResult> GetCVMarkAndStatus(Guid applicationId)
    //{
    //    var result = await _service.GetCVMarkAndStatusAsync(applicationId);

    //    if (result == null)
    //        return NotFound("Application not found");

    //    return Ok(result);
    //}

    //[HttpGet("{applicationId}/cv-status")]
    //public async Task<IActionResult> GetCVMarkAndStatus(Guid applicationId)
    //{
    //    var result = await _service.GetCVMarkAndStatusAsync(applicationId);

    //    if (result == null)
    //        return NotFound("Application not found");

    //    // Send CV mark email here (using minimal data from another DTO)
    //    var cvData = await _service.GetCVMarkAndEmailAsync(applicationId);

    //    if (cvData != null && !string.IsNullOrEmpty(cvData.UserEmail))
    //    {
    //        await _service.SendCVMarkEmailAsync(cvData.UserEmail, cvData.CV_Mark);
    //    }

    //    return Ok(result);
    //}

    [HttpGet("{applicationId}/cv-status")]
    public async Task<IActionResult> GetCVMarkAndStatus(Guid applicationId)
    {
        var result = await _service.GetCVMarkAndStatusAsync(applicationId);

        if (result == null)
            return NotFound("Application not found");

        var cvData = await _service.GetCVMarkAndEmailAsync(applicationId);

        if (cvData != null && !string.IsNullOrEmpty(cvData.UserEmail))
        {
            if (!string.Equals(result.Status, "CV Approved", StringComparison.OrdinalIgnoreCase))
            {
                // Send rejection email
                await _service.SendCVRejectionEmailAsync(cvData.UserEmail, cvData.CV_Mark);
            }
            else
            {
                // Send CV mark email
                await _service.SendCVMarkEmailAsync(cvData.UserEmail, cvData.CV_Mark);
            }
        }

        return Ok(result);
    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckCVExists(Guid userId, Guid vacancyId)
    {
        var (exists, applicationId) = await _service.CheckIfCVExistsAsync(userId, vacancyId);

        return Ok(new { exists, applicationId });
    }



}
