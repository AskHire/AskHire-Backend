using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using fileupload.Interfaces;

namespace fileupload.Repositories
{
    public class CandidateFileRepository : ICandidateFileRepository
    {
        private readonly BlobContainerClient _filesContainer;
        private readonly string _storageAccount = "askhiredatabase";
        private readonly string _key = "LyQZVeduwuh+HA1W2SFHyCPnzo3SaGeJvyt8a1h7AkyDuHAiGo5McKFFiNECnpfVWs5S5Xx5HexX+AStsQZ+KQ=="; 

        public CandidateFileRepository()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = blobServiceClient.GetBlobContainerClient("files");
            _filesContainer.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<List<BlobDto>> ListAsync()
        {
            var files = new List<BlobDto>();
            await foreach (var file in _filesContainer.GetBlobsAsync())
            {
                string fullUri = $"{_filesContainer.Uri}/{file.Name}";
                files.Add(new BlobDto
                {
                    Uri = fullUri,
                    Name = file.Name,
                    ContentType = file.Properties.ContentType
                });
            }
            return files;
        }

        public async Task<BlobResponseDto> UploadAsync(IFormFile file)
        {
            var client = _filesContainer.GetBlobClient(file.FileName);
            await using var stream = file.OpenReadStream();
            await client.UploadAsync(stream, overwrite: true);

            return new BlobResponseDto
            {
                Status = $"File {file.FileName} uploaded successfully",
                Error = false,
                Blob = new BlobDto
                {
                    Uri = client.Uri.AbsoluteUri,
                    Name = file.FileName,
                    ContentType = file.ContentType
                }
            };
        }

        public async Task<BlobDto?> DownloadAsync(string filename)
        {
            var file = _filesContainer.GetBlobClient(filename);
            if (await file.ExistsAsync())
            {
                var content = await file.DownloadContentAsync();
                var data = await file.OpenReadAsync();

                return new BlobDto
                {
                    Content = data,
                    Name = filename,
                    ContentType = content.Value.Details.ContentType
                };
            }
            return null;
        }

        public async Task<BlobResponseDto> DeleteAsync(string filename)
        {
            var file = _filesContainer.GetBlobClient(filename);
            await file.DeleteIfExistsAsync();
            return new BlobResponseDto
            {
                Error = false,
                Status = $"File: {filename} has been successfully deleted."
            };
        }
    }
}
