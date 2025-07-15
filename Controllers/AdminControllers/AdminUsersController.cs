using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs;  // <- Correct DTO namespace
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AdminUserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(
    int Page = 1,
    int PageSize = 5,
    string? SearchTerm = null,
    string? RoleFilter = null,
    string? SortBy = "Id", // default to Id
    bool IsDescending = false)
        {
            var query = _userManager.Users.AsQueryable();

            // Role filter
            if (!string.IsNullOrEmpty(RoleFilter))
                query = query.Where(u => u.Role == RoleFilter);

            // Search by FirstName or LastName
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var lowerSearch = SearchTerm.ToLower();
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(lowerSearch)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(lowerSearch))
                );
            }

            // Sorting
            try
            {
                if (string.IsNullOrEmpty(SortBy))
                {
                    return BadRequest("SortBy field cannot be null or empty.");
                }

                query = IsDescending
                    ? query.OrderByDescending(u => EF.Property<object>(u, SortBy))
                    : query.OrderBy(u => EF.Property<object>(u, SortBy));
            }
            catch (Exception)
            {
                return BadRequest($"Invalid SortBy field: {SortBy}");
            }


            // Total count
            var totalUsers = await query.CountAsync();

            // Pagination
            var users = await query
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Map to DTOs
            var usersDto = users.Select(u => new UserDTo
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role ?? "Candidate",
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                DOB = u.DOB,
                NIC = u.NIC,
                MobileNumber = u.MobileNumber,
                Address = u.Address,
                ProfilePictureUrl = u.ProfilePictureUrl
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalUsers / PageSize);

            return Ok(new
            {
                data = usersDto,
                totalPages = totalPages
            });
        }


        // PUT: api/AdminUser/UpdateRole
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound("User not found.");

            user.Role = request.NewRole;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest("Failed to update role.");

            return Ok(new { message = "User role updated successfully." });
        }

        // DELETE: api/AdminUser/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest("Failed to delete user.");

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
