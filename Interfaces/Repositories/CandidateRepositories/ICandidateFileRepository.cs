using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace fileupload.Interfaces
{
    public interface ICandidateFileRepository
    {
        Task<List<BlobDto>> ListAsync();
        Task<BlobResponseDto> UploadAsync(IFormFile file);
        Task<BlobDto?> DownloadAsync(string filename);
        Task<BlobResponseDto> DeleteAsync(string filename);
    }
}
