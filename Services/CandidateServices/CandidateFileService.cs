using fileupload.Interfaces;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace fileupload.Services
{
    public class CandidateFileService : ICandidateFileService
    {
        private readonly ICandidateFileRepository _fileRepo;
        private readonly AppDbContext _context;

        public CandidateFileService(ICandidateFileRepository fileRepo, AppDbContext context)
        {
            _fileRepo = fileRepo;
            _context = context;
        }

        public async Task<List<BlobDto>> ListFilesAsync()
        {
            return await _fileRepo.ListAsync();
        }

        public async Task<BlobResponseDto> UploadFileAsync(IFormFile file, Guid userId, Guid vacancyId)
        {
            var uploadResult = await _fileRepo.UploadAsync(file);

            var user = await _context.Users.FindAsync(userId);
            var vacancy = await _context.Vacancies.FindAsync(vacancyId);

            if (user == null || vacancy == null)
            {
                throw new ArgumentException("Invalid UserId or VacancyId");
            }

            var application = new Application
            {
                ApplicationId = Guid.NewGuid(),
                UserId = userId,
                VacancyId = vacancyId,
                User = user,
                Vacancy = vacancy,
                CVFilePath = uploadResult.Blob.Uri ?? string.Empty,
                CV_Mark = 0,
                Pre_Screen_PassMark = 0,
                Status = "Pending",
                DashboardStatus = "Applied"
            };

            _context.Applies.Add(application);
            await _context.SaveChangesAsync();

            return uploadResult;
        }


        public async Task<BlobDto?> DownloadFileAsync(string filename)
        {
            return await _fileRepo.DownloadAsync(filename);
        }

        public async Task<BlobResponseDto> DeleteFileAsync(string filename)
        {
            return await _fileRepo.DeleteAsync(filename);
        }

        public async Task<BlobDto?> DownloadFileByApplicationIdAsync(Guid applicationId)
        {
            var application = await _context.Applies.FindAsync(applicationId);
            if (application == null || string.IsNullOrEmpty(application.CVFilePath))
                throw new ArgumentException("Invalid application ID or no file path found.");

            var filename = Path.GetFileName(new Uri(application.CVFilePath).LocalPath);
            return await _fileRepo.DownloadAsync(filename);
        }

        public async Task<BlobResponseDto> DeleteFileByApplicationIdAsync(Guid applicationId)
        {
            var application = await _context.Applies.FindAsync(applicationId);
            if (application == null)
                throw new ArgumentException("Invalid application ID.");

            if (string.IsNullOrEmpty(application.CVFilePath))
                throw new InvalidOperationException("No file associated with this application.");

            var filename = Path.GetFileName(new Uri(application.CVFilePath).LocalPath);

            // Delete from Azure Blob
            var deleteResult = await _fileRepo.DeleteAsync(filename);

            // Update DB - Clear file path
            application.CVFilePath = null;
            _context.Applies.Update(application); // Optional, EF Core tracks by default
            await _context.SaveChangesAsync();    // Ensure this is awaited

            return deleteResult;
        }


    }
}
