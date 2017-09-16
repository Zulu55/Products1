namespace Products1.ViewModels
{
    public class MainViewModel
    {
        #region Properties
        public LoginViewModel Login
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            Login = new LoginViewModel();
        }
        #endregion
    }
}
