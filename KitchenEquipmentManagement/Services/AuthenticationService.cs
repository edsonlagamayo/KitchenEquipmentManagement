using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Models;

namespace KitchenEquipmentManagement.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private User? _currentUser;

        public AuthenticationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User? CurrentUser => _currentUser;

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

                if (user != null && VerifyPassword(password, user.Password))
                {
                    _currentUser = user;
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            try
            {
                // Check if username or email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName.ToLower() == user.UserName.ToLower() ||
                                            u.EmailAddress.ToLower() == user.EmailAddress.ToLower());

                if (existingUser != null)
                {
                    throw new Exception("Username or email already exists.");
                }

                // Hash the password
                user.Password = HashPassword(password);
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return !await _context.Users
                .AnyAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _context.Users
                .AnyAsync(u => u.EmailAddress.ToLower() == email.ToLower());
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        public void SetCurrentUser(User? user)
        {
            _currentUser = user;
        }

        public void Logout()
        {
            _currentUser = null;
        }
    }
}