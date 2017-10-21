namespace Products1.ViewModels
{
    using System.ComponentModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Services;
    using Xamarin.Forms;

    public class PasswordRecoveryViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        ApiService apiService;
        DialogService dialogService;
        NavigationService navigationService;
        #endregion

        #region Attributes
        bool _isRunning;
        bool _isEnabled;
        #endregion

        #region Properties
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public string Email
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public PasswordRecoveryViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();

            IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        async void Save()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter a email.");
                return;
            }

            if (!RegexUtilities.IsValidEmail(Email))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter a valid email.");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();

            var response = await apiService.PasswordRecovery(
                urlAPI,
                "/api",
                "/Customers/PasswordRecovery",
                Email);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage(
                    "Error",
                    "We can't send the new password to this email.");
                return;
            }

            await dialogService.ShowMessage(
                "Confirm",
                "Your new password has been sent to your email!");
            await navigationService.BackOnLogin();

            IsRunning = false;
            IsEnabled = true;
        }
        #endregion
    }
}
