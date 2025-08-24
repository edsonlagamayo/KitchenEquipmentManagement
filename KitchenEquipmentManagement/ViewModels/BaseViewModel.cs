using System.ComponentModel;
using System.Runtime.CompilerServices;
using RelayCommand = KitchenEquipmentManagement.Utilities.RelayCommand;

namespace KitchenEquipmentManagement.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        protected void ClearError()
        {
            ErrorMessage = string.Empty;
        }

        protected void ShowError(string message)
        {
            ErrorMessage = message;
        }

        protected async Task ExecuteAsync(Func<Task> operation)
        {
            try
            {
                IsLoading = true;
                ClearError();
                await operation();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            try
            {
                IsLoading = true;
                ClearError();
                return await operation();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return default;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}