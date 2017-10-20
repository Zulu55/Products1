namespace Products1.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Services;
    using Xamarin.Forms;

    public class SyncViewModel : INotifyPropertyChanged
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
        string _message;
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

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Message)));
                }
            }
        }
        #endregion

        #region Constructors
        public SyncViewModel()
        {
            apiService = new ApiService();
            dataService = new DataService();
            dialogService = new DialogService();
            navigationService = new NavigationService();

            Message = "Press sync button to start";
            IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand SyncCommand
        {
            get
            {
                return new RelayCommand(Sync);
            }
        }

        async void Sync()
        {
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

            var produts = dataService.Get<Product>(false)
                                     .Where(p => p.PendingToSave)
                                     .ToList();

            if (produts.Count == 0)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "There are not products to sync.");
                return;
            }

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();
            var mainViewModel = MainViewModel.GetInstance();

            foreach (var product in produts)
            {
                var response = await apiService.Post(
                    urlAPI,
                    "/api",
                    "/Products",
                    mainViewModel.Token.TokenType,
                    mainViewModel.Token.AccessToken,
                    product);
                if (response.IsSuccess)
                {
                    product.PendingToSave = false;
                    dataService.Update(product);
                }
            }

            IsRunning = false;
            IsEnabled = true;
            await dialogService.ShowMessage("Confimation", "Sync Ok");
            await navigationService.BackOnMaster();
        }
        #endregion
    }
}
