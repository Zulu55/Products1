namespace Products1.Models
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Xamarin.Forms;
    using Views;
    using ViewModels;

    public class Category
    {
        #region Properties
        public int CategoryId { get; set; }

        public string Description { get; set; }

        public List<Product> Products { get; set; }
        #endregion

        #region Commands
        public ICommand SelectCategoryCommand
        {
            get
            {
                return new RelayCommand(SelectCategory);
            }
        }

        async void SelectCategory()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Products = new ProductsViewModel(Products);
            await Application.Current.MainPage.Navigation.PushAsync(
                new ProductsView());
        }
        #endregion
    }
}
