using TestMVC.Data;
using TestMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestMVC.Services
{
    public class UsersService
    {
        private readonly TestMVCContext _context;
        private readonly ILogger<UsersService> _logger;

        public UsersService(TestMVCContext context, ILogger<UsersService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get all users asynchronously
        public async Task<List<User>> GetUserAsync()
        {
            try
            {
                return await _context.User.Where(user => user.DeletedAt == null).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return new List<User>(); // Return an empty list on error
            }
        }

        // Get a single user by Id asynchronously
        public async Task<User> GetUserAsync(int id)
        {
            try
            {
                return await _context.User.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user with ID {id}");
                return null;
            }
        }

        // Create a new user
        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return false; // Indicate failure
            }
        }

        // Edit or update an existing user
        public async Task<bool> EditUserAsync(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return false; // Indicate failure
            }
        }

        // Remove a user by Id
        public async Task<bool> RemoveUserAsync(int id)
        {
            try
            {
                User user = await _context.User.FirstOrDefaultAsync(x => x.Id == id);
                if (user != null)
                {
                    user.DeletedAt = DateTime.UtcNow;
                    _context.User.Update(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false; // User not found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user with ID {id}");
                return false; // Indicate failure
            }
        }
    }
}
