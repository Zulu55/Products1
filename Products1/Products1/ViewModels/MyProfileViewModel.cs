namespace Products1.ViewModels
{
    using System.ComponentModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Services;
    using Xamarin.Forms;

    public class MyProfileViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        ApiService apiService;
        DataService dataService;
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

        public string CurrentPassword
        {
            get;
            set;
        }


        public string NewPassword
        {
            get;
            set;
        }

        public string ConfirmPassword
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public MyProfileViewModel()
        {
            apiService = new ApiService();
            dataService = new DataService();
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
            if (string.IsNullOrEmpty(CurrentPassword))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter the current password.");
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();

            if (!mainViewModel.Token.Password.Equals(CurrentPassword))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "The current password is not valid");
                return;
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter a new password.");
                return;
            }

            if (NewPassword.Length < 6)
            {
                await dialogService.ShowMessage(
                    "Error",
                    "The new password must have at least 6 characters length.");
                return;
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter a new password confirm.");
                return;
            }

            if (!NewPassword.Equals(ConfirmPassword))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "The new password and confirm, does not match.");
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

            var changePasswordRequest = new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                Email = mainViewModel.Token.UserName,
                NewPassword = NewPassword,
            };

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();

            var response = await apiService.ChangePassword(
                urlAPI,
                "/api",
                "/Customers/ChangePassword",
                mainViewModel.Token.TokenType,
                mainViewModel.Token.AccessToken,
                changePasswordRequest);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            mainViewModel.Token.Password = NewPassword;
            dataService.Update(mainViewModel.Token);

            await dialogService.ShowMessage(
                "Confirm",
                "The password was changed successfully");
            await navigationService.BackOnMaster();

            IsRunning = false;
            IsEnabled = true;
        }
        #endregion
    }
}
