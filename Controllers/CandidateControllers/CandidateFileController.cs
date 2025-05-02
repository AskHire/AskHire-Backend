using fileupload.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fileupload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateFileController : ControllerBase
    {
        private readonly ICandidateFileService _fileService;

        public CandidateFileController(ICandidateFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> ListAllBlobs()
        {
            var result = await _fileService.ListFilesAsync();
            return Ok(result);
        }

        [HttpPost("{userId:guid}/{vacancyId:guid}")]
        public async Task<IActionResult> Upload(IFormFile file, Guid userId, Guid vacancyId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or not provided.");

            var result = await _fileService.UploadFileAsync(file, userId, vacancyId);
            return Ok(result);
        }

        [HttpGet("filename")]
        public async Task<IActionResult> Download(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return BadRequest("Filename is required.");

            var result = await _fileService.DownloadFileAsync(filename);
            if (result == null)
                return NotFound(new { message = $"File '{filename}' not found." });

            return File(result.Content, result.ContentType, result.Name);
        }

        [HttpDelete("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return BadRequest("Filename is required.");

            var result = await _fileService.DeleteFileAsync(filename);
            return Ok(result);
        }
        [HttpGet("download-by-application/{applicationId:guid}")]
        public async Task<IActionResult> DownloadByApplicationId(Guid applicationId)
        {
            try
            {
                var result = await _fileService.DownloadFileByApplicationIdAsync(applicationId);
                if (result == null)
                    return NotFound($"No file found for Application ID: {applicationId}");

                return File(result.Content, result.ContentType, result.Name);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-by-application/{applicationId:guid}")]
        public async Task<IActionResult> DeleteByApplicationId(Guid applicationId)
        {
            try
            {
                var result = await _fileService.DeleteFileByApplicationIdAsync(applicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
