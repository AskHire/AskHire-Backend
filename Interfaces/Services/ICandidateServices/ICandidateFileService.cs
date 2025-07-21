//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//public interface ICandidateFileService
//{
//    Task<IActionResult> UploadCvAsync(Guid userId, Guid vacancyId, IFormFile file);
//    Task<IActionResult> DownloadCvAsync(Guid applicationId);
//    Task<IActionResult> DeleteCvAsync(Guid applicationId);
//    Task<IActionResult> ViewUploadedCvsAsync();
//}
using System;
using System.Threading.Tasks;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public interface ICandidateFileService
{
    Task<IActionResult> UploadCvAsync(Guid userId, Guid vacancyId, IFormFile file);
    Task<IActionResult> DownloadCvAsync(Guid applicationId);
    Task<IActionResult> DeleteCvAsync(Guid applicationId);
    Task<IActionResult> ViewUploadedCvsAsync();
    Task<IActionResult> AnalyzeApplicationAsync(Guid applicationId);
    //Task<int?> GetCVMarkAsync(Guid applicationId);
    Task<ApplicationCVStatusDto?> GetCVMarkAndStatusAsync(Guid applicationId);
    Task<ApplicationCVMarkDto?> GetCVMarkAndEmailAsync(Guid applicationId);
    Task<bool> SendCVMarkEmailAsync(string recipientEmail, int? cvMark);
    Task<bool> SendCVRejectionEmailAsync(string recipientEmail, int? cvMark);
    Task<(bool Exists, Guid? ApplicationId)> CheckIfCVExistsAsync(Guid userId, Guid vacancyId);

}
