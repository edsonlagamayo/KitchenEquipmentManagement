using KitchenEquipmentManagement.Models;

namespace KitchenEquipmentManagement.Services
{
    public interface IAuthenticationService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        User? CurrentUser { get; }
        void SetCurrentUser(User? user);
        void Logout();
    }
}