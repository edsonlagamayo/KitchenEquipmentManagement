using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using KitchenEquipmentManagement.Data;
using KitchenEquipmentManagement.Models;
using KitchenEquipmentManagement.Services;
using KitchenEquipmentManagement.Utilities;
using RelayCommand = KitchenEquipmentManagement.Utilities.RelayCommand;

namespace KitchenEquipmentManagement.ViewModels
{
    public class UserMaintenanceViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationService _authService;
        private ObservableCollection<User> _users = new();
        private User? _selectedUser;
        private bool _isEditMode;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _emailAddress = string.Empty;
        private string _userName = string.Empty;
        private string _selectedUserType = "Admin";
        private string _password = string.Empty;

        public UserMaintenanceViewModel(ApplicationDbContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;

            EditCommand = new RelayCommand(EditUser, CanEditUser);
            DeleteCommand = new RelayCommand(async () => await DeleteUserAsync(), CanDeleteUser);
            SaveCommand = new RelayCommand(async () => await SaveUserAsync(), CanSaveUser);
            CancelCommand = new RelayCommand(CancelEdit);

            UserTypes = new List<string> { "Admin", "SuperAdmin" };

            _ = LoadUsersAsync();
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value);
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                SetProperty(ref _emailAddress, value);
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string SelectedUserType
        {
            get => _selectedUserType;
            set => SetProperty(ref _selectedUserType, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public List<string> UserTypes { get; }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadUsersAsync()
        {
            await ExecuteAsync(async () =>
            {
                var users = await _context.Users
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToListAsync();

                Users = new ObservableCollection<User>(users);
            });
        }

        private bool CanEditUser()
        {
            return SelectedUser != null && !IsLoading;
        }

        private bool CanDeleteUser()
        {
            return SelectedUser != null && !IsLoading &&
                   SelectedUser.UserId != _authService.CurrentUser?.UserId; // Can't delete self
        }

        private bool CanSaveUser()
        {
            return IsEditMode && !IsLoading &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(EmailAddress) &&
                   !string.IsNullOrWhiteSpace(UserName);
        }

        private void EditUser()
        {
            if (SelectedUser != null)
            {
                FirstName = SelectedUser.FirstName;
                LastName = SelectedUser.LastName;
                EmailAddress = SelectedUser.EmailAddress;
                UserName = SelectedUser.UserName;
                SelectedUserType = SelectedUser.UserType;
                Password = string.Empty;
                IsEditMode = true;
            }
        }

        private async Task DeleteUserAsync()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete user '{SelectedUser.FullName}'?\n\nThis will also delete all associated sites and equipment.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await ExecuteAsync(async () =>
                {
                    _context.Users.Remove(SelectedUser);
                    await _context.SaveChangesAsync();
                    await LoadUsersAsync();
                    SelectedUser = null;
                });
            }
        }

        private async Task SaveUserAsync()
        {
            if (SelectedUser == null) return;

            await ExecuteAsync(async () =>
            {
                // Check if username or email changed and is available
                if (UserName != SelectedUser.UserName)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserName.ToLower() == UserName.ToLower() && u.UserId != SelectedUser.UserId);
                    if (existingUser != null)
                    {
                        ShowError("Username is already taken.");
                        return;
                    }
                }

                if (EmailAddress != SelectedUser.EmailAddress)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.EmailAddress.ToLower() == EmailAddress.ToLower() && u.UserId != SelectedUser.UserId);
                    if (existingUser != null)
                    {
                        ShowError("Email address is already registered.");
                        return;
                    }
                }

                // Update user properties
                SelectedUser.FirstName = FirstName.Trim();
                SelectedUser.LastName = LastName.Trim();
                SelectedUser.EmailAddress = EmailAddress.Trim().ToLower();
                SelectedUser.UserName = UserName.Trim();
                SelectedUser.UserType = SelectedUserType;
                SelectedUser.ModifiedDate = DateTime.Now;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    if (Password.Length < 6)
                    {
                        ShowError("Password must be at least 6 characters long.");
                        return;
                    }
                    SelectedUser.Password = _authService.HashPassword(Password);
                }

                await _context.SaveChangesAsync();
                await LoadUsersAsync();

                IsEditMode = false;
                ClearForm();

                MessageBox.Show("User updated successfully!", "Success",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            ClearForm();
        }

        private void ClearForm()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            EmailAddress = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            SelectedUserType = "Admin";
        }
    }
}