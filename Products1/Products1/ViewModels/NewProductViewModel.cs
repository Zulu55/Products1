namespace Products1.ViewModels
{
    using System;
    using System.ComponentModel;
	using System.Windows.Input;
	using GalaSoft.MvvmLight.Command;
    using Helpers;
	using Models;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Services;
    using Xamarin.Forms;

    public class NewProductViewModel : INotifyPropertyChanged
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
        ImageSource _imageSource;
		MediaFile file;
		#endregion

		#region Properties
        public ImageSource ImageSource
		{
			set
			{
				if (_imageSource != value)
				{
					_imageSource = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(ImageSource)));
				}
			}
			get
			{
				return _imageSource;
			}
		}

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
		public NewProductViewModel()
		{
			apiService = new ApiService();
			dialogService = new DialogService();
			navigationService = new NavigationService();

            ImageSource = "noimage";
            IsActive = true;
            LastPurchase = DateTime.Today;

			IsEnabled = true;
		}
		#endregion

		#region Commands
        public ICommand ChangeImageCommand
		{
			get
			{
				return new RelayCommand(ChangeImage);
			}
		}

		async void ChangeImage()
		{
			await CrossMedia.Current.Initialize();

			if (CrossMedia.Current.IsCameraAvailable &&
				CrossMedia.Current.IsTakePhotoSupported)
			{
				var source = await dialogService.ShowImageOptions();

				if (source == "Cancel")
				{
					file = null;
					return;
				}

				if (source == "From Camera")
				{
					file = await CrossMedia.Current.TakePhotoAsync(
						new StoreCameraMediaOptions
						{
							Directory = "Sample",
							Name = "test.jpg",
							PhotoSize = PhotoSize.Small,
						}
					);
				}
				else
				{
					file = await CrossMedia.Current.PickPhotoAsync();
				}
			}
			else
			{
				file = await CrossMedia.Current.PickPhotoAsync();
			}

			if (file != null)
			{
				ImageSource = ImageSource.FromStream(() =>
				{
					var stream = file.GetStream();
					return stream;
				});
			}
		}

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

            byte[] imageArray = null;
			if (file != null)
			{
				imageArray = FilesHelper.ReadFully(file.GetStream());
				file.Dispose();
			}

			var mainViewModel = MainViewModel.GetInstance();

			var product = new Product
			{
				CategoryId = mainViewModel.Category.CategoryId,
				Description = Description,
				ImageArray = imageArray,
				IsActive = IsActive,
				LastPurchase = LastPurchase,
				Price = price,
				Remarks = Remarks,
				Stock = stock,
			};

			var response = await apiService.Post(
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

			product = (Product)response.Result;
			var productsViewModel = ProductsViewModel.GetInstance();
			productsViewModel.Add(product);

			await navigationService.BackOnMaster();

			IsRunning = false;
			IsEnabled = true;
		}
		#endregion
	}
}
