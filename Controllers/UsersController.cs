using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return user;
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            
                user.UserId = Guid.NewGuid();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, User user)
        {
            if (id != user.UserId)
                return BadRequest("User ID mismatch");

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            // Update all fields (can be adjusted to update specific ones only)
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Gender = user.Gender;
            existingUser.DOB = user.DOB;
            existingUser.NIC = user.NIC;
            existingUser.MobileNumber = user.MobileNumber;
            existingUser.Address = user.Address;
            existingUser.State = user.State;
            existingUser.Role = user.Role;
            existingUser.RefreshToken = user.RefreshToken;
            existingUser.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Optional: PATCH only the Role (used if role change is frequent)
        [HttpPatch("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] string newRole)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
