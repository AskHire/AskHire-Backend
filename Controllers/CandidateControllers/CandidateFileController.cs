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

    [HttpGet("{applicationId}/cv-status")]
    public async Task<IActionResult> GetCVMarkAndStatus(Guid applicationId)
    {
        var result = await _service.GetCVMarkAndStatusAsync(applicationId);

        if (result == null)
            return NotFound("Application not found");

        // Send CV mark email here (using minimal data from another DTO)
        var cvData = await _service.GetCVMarkAndEmailAsync(applicationId);

        if (cvData != null && !string.IsNullOrEmpty(cvData.UserEmail))
        {
            await _service.SendCVMarkEmailAsync(cvData.UserEmail, cvData.CV_Mark);
        }

        return Ok(result);
    }


    //[HttpPost("{applicationId}/send-cv-mark-email")]
    //public async Task<IActionResult> SendCVMarkEmail(Guid applicationId)
    //{
    //    var cvData = await _service.GetCVMarkAndEmailAsync(applicationId);

    //    if (cvData == null || string.IsNullOrEmpty(cvData.UserEmail))
    //        return NotFound("Candidate not found or email missing.");

    //    bool result = await _service.SendCVMarkEmailAsync(cvData.UserEmail, cvData.CV_Mark);

    //    if (result)
    //        return Ok("CV mark email sent successfully.");
    //    else
    //        return StatusCode(500, "Failed to send email.");
    //}

}
