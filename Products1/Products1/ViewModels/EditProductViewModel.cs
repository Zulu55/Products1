namespace Products1.ViewModels
{
	using System;
	using System.ComponentModel;
	using System.Windows.Input;
	using GalaSoft.MvvmLight.Command;
	using Models;
	using Services;

	public class EditProductViewModel: INotifyPropertyChanged
    {
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Services
		ApiService apiService;
		DialogService dialogService;
		NavigationService navigationService;
		#endregion

		#region Attributes
		bool _isRunning;
		bool _isEnabled;
        Product product;
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

		public string Description
		{
			get;
			set;
		}

		public string Price
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		public DateTime LastPurchase
		{
			get;
			set;
		}

		public string Stock
		{
			get;
			set;
		}

		public string Remarks
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}
		#endregion

		#region Constructors
		public EditProductViewModel(Product product)
		{
            this.product = product;

			apiService = new ApiService();
			dialogService = new DialogService();
			navigationService = new NavigationService();

            Description = product.Description;
            Image = product.ImageFullPath;
            Price = product.Price.ToString();
			IsActive = product.IsActive;
			LastPurchase = product.LastPurchase;
            Stock = product.Stock.ToString();
            Remarks = product.Remarks;

			IsEnabled = true;
		}
		#endregion

		#region Commands
		public ICommand SaveCommand
		{
			get
			{
				return new RelayCommand(Save);
			}
		}

		async void Save()
		{
			if (string.IsNullOrEmpty(Description))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a product description.");
				return;
			}

			if (string.IsNullOrEmpty(Price))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a product price.");
				return;
			}

			var price = decimal.Parse(Price);
			if (price < 0)
			{
				await dialogService.ShowMessage(
					"Error",
					"The price must be a value greather or equals than zero.");
				return;
			}

			if (string.IsNullOrEmpty(Stock))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a product stock.");
				return;
			}

			var stock = double.Parse(Stock);
			if (stock < 0)
			{
				await dialogService.ShowMessage(
					"Error",
					"The stock must be a value greather or equals than zero.");
				return;
			}

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

			var mainViewModel = MainViewModel.GetInstance();

            product.Description = Description;
            product.IsActive = IsActive;
            product.LastPurchase = LastPurchase;
            product.Price = price;
            product.Remarks = Remarks;
            product.Stock = stock;

			var response = await apiService.Put(
				"http://productszuluapi.azurewebsites.net",
				"/api",
				"/Products",
				mainViewModel.Token.TokenType,
				mainViewModel.Token.AccessToken,
				product);

			if (!response.IsSuccess)
			{
				IsRunning = false;
				IsEnabled = true;
				await dialogService.ShowMessage(
					"Error",
					response.Message);
				return;
			}

			ProductsViewModel.GetInstance().Update(product);

			await navigationService.Back();

			IsRunning = false;
			IsEnabled = true;
		}
		#endregion
	}
}
