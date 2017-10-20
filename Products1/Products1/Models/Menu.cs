namespace Products1.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using ViewModels;
    using Xamarin.Forms;

    public class Menu
    {
        #region Services
        ApiService apiService;
        DataService dataService; 
        DialogService dialogService; 
        NavigationService navigationService; 
        #endregion

        #region Properties
        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageName { get; set; }
        #endregion

        #region Constructors
        public Menu()
        {
            apiService = new ApiService();
            dataService = new DataService();
            navigationService = new NavigationService();
        }
        #endregion

        #region Commands
        public ICommand NavigateCommand
        {
            get
            {
                return new RelayCommand(Navigate);
            }
        }

        async void Navigate()
        {
            switch (PageName)
            {
                case "LoginView":
                    var mainViewModel = MainViewModel.GetInstance();
                    mainViewModel.Token.IsRemembered = false;
                    dataService.Update(mainViewModel.Token);
                    mainViewModel.Login = new LoginViewModel();
                    navigationService.SetMainPage("LoginView");
                    break;
                case "UbicationsView":
                    MainViewModel.GetInstance().Ubications = 
                        new UbicationsViewModel();
                    await navigationService.NavigateOnMaster("UbicationsView");
                    break;
                case "SyncView":
                    SyncData();
                    break;
            }
        }

        async void SyncData()
        {
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var produts = dataService.Get<Product>(false).
                                     Where(p => p.PendingToSave).
                                     ToList();

            if (produts.Count == 0)
            {
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

            await dialogService.ShowMessage("Confimation", "Sync Ok");
        }
        #endregion
    }
}
