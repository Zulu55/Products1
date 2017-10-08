namespace Products1.Models
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using ViewModels;

    public class Category
    {
		#region Services
        DialogService dialogService;
		NavigationService navigationService;
		#endregion

		#region Properties
		public int CategoryId { get; set; }

        public string Description { get; set; }

        public List<Product> Products { get; set; }
        #endregion

        #region Constructors
        public Category()
        {
            dialogService = new DialogService();
            navigationService = new NavigationService();
        }
        #endregion

        #region Methods
        public override int GetHashCode()
        {
            return CategoryId;
        }
        #endregion

        #region Commands
        public ICommand DeleteCommand
        {
			get
			{
				return new RelayCommand(Delete);
			}
		}

        async void Delete()
        {
            var response = await dialogService.ShowConfirm(
                "Confirm", 
                "Are you sure to delete this record?");
            if (!response)
            {
                return;
            }

            await CategoriesViewModel.GetInstance().Delete(this);
        }

        public ICommand EditCommand
        {
			get
			{
				return new RelayCommand(Edit);
			}
		}

        async void Edit()
        {
            MainViewModel.GetInstance().EditCategory = 
                new EditCategoryViewModel(this);
            await navigationService.Navigate("EditCategoryView");
        }

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
            mainViewModel.Category = this;
            await navigationService.Navigate("ProductsView");
        }
        #endregion
    }
}
