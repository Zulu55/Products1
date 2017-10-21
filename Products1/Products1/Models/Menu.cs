namespace Products1.Models
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using ViewModels;

    public class Menu
    {
        #region Services
        DataService dataService; 
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
                    navigationService.SetMainPage(PageName);
                    break;
                case "UbicationsView":
                    MainViewModel.GetInstance().Ubications = 
                        new UbicationsViewModel();
                    await navigationService.NavigateOnMaster(PageName);
                    break;
                case "SyncView":
                    MainViewModel.GetInstance().Sync = new SyncViewModel();
                    await navigationService.NavigateOnMaster(PageName);
                    break;
                case "MyProfileView":
                    MainViewModel.GetInstance().MyProfile = 
                        new MyProfileViewModel();
                    await navigationService.NavigateOnMaster(PageName);
                    break;
            }
        }
        #endregion
    }
}
