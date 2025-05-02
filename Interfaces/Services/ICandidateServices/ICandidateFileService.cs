using Microsoft.AspNetCore.Http;

namespace fileupload.Interfaces
{
    public interface ICandidateFileService
    {
        Task<List<BlobDto>> ListFilesAsync();
        Task<BlobResponseDto> UploadFileAsync(IFormFile file, Guid userId, Guid vacancyId);
        Task<BlobDto?> DownloadFileAsync(string filename);
        Task<BlobResponseDto> DeleteFileAsync(string filename);
        Task<BlobDto?> DownloadFileByApplicationIdAsync(Guid applicationId);
        Task<BlobResponseDto> DeleteFileByApplicationIdAsync(Guid applicationId);

    }
}
