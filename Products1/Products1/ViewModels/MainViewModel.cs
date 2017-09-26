namespace Products1.ViewModels
{
    using Models;

    public class MainViewModel
    {
        #region Properties
        public LoginViewModel Login
        {
            get;
            set;
        }

        public CategoriesViewModel Categories
        {
            get;
            set;
        }

        public  TokenResponse Token
        {
            get;
            set;
   
        }

        public ProductsViewModel Products
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            instance = this;

            Login = new LoginViewModel();
        }
        #endregion

        #region Sigleton
        static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }
        #endregion
    }
}
