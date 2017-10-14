namespace Products1.ViewModels
{
	using System.ComponentModel;
	using System.Windows.Input;
	using GalaSoft.MvvmLight.Command;
	using Helpers;
	using Models;
	using Services;

	public class NewCustomerViewModel : INotifyPropertyChanged
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

		public string FirstName
		{
			get;
			set;
		}


		public string LastName
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public string Phone
		{
			get;
			set;
		}

		public string Address
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string Confirm
		{
			get;
			set;
		}
		#endregion

		#region Constructors
		public NewCustomerViewModel()
		{
			apiService = new ApiService();
			dialogService = new DialogService();
			navigationService = new NavigationService();

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
			if (string.IsNullOrEmpty(FirstName))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a first name.");
				return;
			}

			if (string.IsNullOrEmpty(LastName))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a last name.");
				return;
			}

			if (string.IsNullOrEmpty(Email))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a email.");
				return;
			}

			if (!RegexUtilities.IsValidEmail(Email))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a valid email.");
				return;
			}

			if (string.IsNullOrEmpty(Password))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a password.");
				return;
			}

			if (Password.Length < 6)
			{
				await dialogService.ShowMessage(
					"Error",
					"The password must have at least 6 characters length.");
				return;
			}

			if (string.IsNullOrEmpty(Confirm))
			{
				await dialogService.ShowMessage(
					"Error",
					"You must enter a password confirm.");
				return;
			}

			if (!Password.Equals(Confirm))
			{
				await dialogService.ShowMessage(
					"Error",
					"The password and confirm, does not match.");
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

			var customer = new Customer
			{
				Address = Address,
				CustomerType = 1,
				Email = Email,
				FirstName = FirstName,
				LastName = LastName,
				Password = Password,
				Phone = Phone,
			};

			var response = await apiService.Post(
				"http://productszuluapi.azurewebsites.net",
				"/api",
				"/Customers",
				customer);

			if (!response.IsSuccess)
			{
				IsRunning = false;
				IsEnabled = true;
				await dialogService.ShowMessage(
					"Error",
					response.Message);
				return;
			}

			var response2 = await apiService.GetToken(
				"http://productszuluapi.azurewebsites.net",
				Email,
				Password);

			if (response2 == null)
			{
				IsRunning = false;
				IsEnabled = true;
				await dialogService.ShowMessage(
					"Error",
					"The service is not available, please try latter.");
				Password = null;
				return;
			}

			if (string.IsNullOrEmpty(response2.AccessToken))
			{
				IsRunning = false;
				IsEnabled = true;
				await dialogService.ShowMessage(
					"Error",
					response2.ErrorDescription);
				Password = null;
				return;
			}

			var mainViewModel = MainViewModel.GetInstance();
			mainViewModel.Token = response2;
			mainViewModel.Categories = new CategoriesViewModel();
            await navigationService.BackOnLogin();
			navigationService.SetMainPage("MasterView");

			IsRunning = false;
			IsEnabled = true;
		}
		#endregion

	}
}
