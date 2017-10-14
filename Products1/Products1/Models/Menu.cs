namespace Products1.Models
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using ViewModels;

    public class Menu
    {
        #region Services
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
                    MainViewModel.GetInstance().Login = 
                        new LoginViewModel();
                    navigationService.SetMainPage("LoginView");
                    break;
                case "UbicationsView":
                    MainViewModel.GetInstance().Ubications = 
                        new UbicationsViewModel();
                    await navigationService.NavigateOnMaster("UbicationsView");
                    break;
            }
        }
        #endregion
    }
}
