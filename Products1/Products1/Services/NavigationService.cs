namespace Products1.Services
{
    using System.Threading.Tasks;
    using Views;
    using Xamarin.Forms;

    public class NavigationService
    {
        public void SetMainPage(string pageName)
        {
            switch (pageName)
            {
                case "LoginView":
                    Application.Current.MainPage = new NavigationPage(new LoginView());
                    break;
                case "MasterView":
                    Application.Current.MainPage = new MasterView();
                    break;
            }
        }

        public async Task NavigateOnMaster(string pageName)
        {
            App.Master.IsPresented = false;

            switch (pageName)
            {
                case "CategoriesView":
                    await App.Navigator.PushAsync(
                        new CategoriesView());
                    break;
                case "ProductsView":
                    await App.Navigator.PushAsync(
                        new ProductsView());
                    break;
                case "NewCategoryView":
                    await App.Navigator.PushAsync(
                        new NewCategoryView());
                    break;
                case "EditCategoryView":
                    await App.Navigator.PushAsync(
                        new EditCategoryView());
                    break;
                case "NewProductView":
                    await App.Navigator.PushAsync(
                        new NewProductView());
                    break;
                case "EditProductView":
                    await App.Navigator.PushAsync(
                        new EditProductView());
                    break;
                case "UbicationsView":
                    await App.Navigator.PushAsync(
                        new UbicationsView());
                    break;
            }
        }

        public async Task NavigateOnLogin(string pageName)
        {
            switch (pageName)
            {
                case "NewCustomerView":
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new NewCustomerView());
                    break;
            }
        }

        public async Task BackOnMaster()
        {
            await App.Navigator.PopAsync();
        }

        public async Task BackOnLogin()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}