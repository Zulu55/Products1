namespace Products1.Models
{
    using System;
	using System.Windows.Input;
	using GalaSoft.MvvmLight.Command;
	using Services;
	using ViewModels;

	public class Product
    {
		#region Services
		DialogService dialogService;
		NavigationService navigationService;
		#endregion

		#region Properties
		public int ProductId { get; set; }

		public int CategoryId { get; set; }

		public string Description { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastPurchase { get; set; }

        public double Stock { get; set; }

        public string Remarks { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(Image))
                {
                    return "noimage";    
                }

                return string.Format(
                    "http://productszulu.azurewebsites.net/{0}", 
                    Image.Substring(1));
            }
        }
		#endregion

		#region Constructors
		public Product()
		{
			dialogService = new DialogService();
			navigationService = new NavigationService();
		}
		#endregion

		#region Methods
		public override int GetHashCode()
		{
            return ProductId;
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

			await ProductsViewModel.GetInstance().Delete(this);
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
            MainViewModel.GetInstance().EditProduct = 
                new EditProductViewModel(this);
			await navigationService.Navigate("EditProductView");
		}
		#endregion
	}
}
