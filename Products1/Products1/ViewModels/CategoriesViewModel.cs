namespace Products1.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using Services;

    public class CategoriesViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        List<Category> categories;
        ObservableCollection<Category> _categories;
        bool _isRefreshing;
        string _filter; 
        #endregion

        #region Properties
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    Search();
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Filter)));
                }
            }
        } 

        public ObservableCollection<Category> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Categories)));
                }
            }
        }

		public bool IsRefreshing
		{
			get
			{
				return _isRefreshing;
			}
			set
			{
				if (_isRefreshing != value)
				{
					_isRefreshing = value;
					PropertyChanged?.Invoke(
						this,
						new PropertyChangedEventArgs(nameof(IsRefreshing)));
				}
			}
		}
		#endregion

		#region Constructors
		public CategoriesViewModel()
        {
            instance = this;

            apiService = new ApiService();
            dialogService = new DialogService();

            LoadCategories();
        }
		#endregion

		#region Sigleton
		static CategoriesViewModel instance;

		public static CategoriesViewModel GetInstance()
		{
			if (instance == null)
			{
				return new CategoriesViewModel();
			}

			return instance;
		}
		#endregion

		#region Methods
		public void Add(Category category)
		{
			IsRefreshing = true;
			categories.Add(category);
			Categories = new ObservableCollection<Category>(
				categories.OrderBy(c => c.Description));
            IsRefreshing = false;
		}

		public void Update(Category category)
		{
			IsRefreshing = true;
			var oldCategory = categories
                .Where(c => c.CategoryId == category.CategoryId)
                .FirstOrDefault();
            oldCategory = category;
			Categories = new ObservableCollection<Category>(
				categories.OrderBy(c => c.Description));
			IsRefreshing = false;
		}

		public async Task Delete(Category category)
		{
			IsRefreshing = true;

			var connection = await apiService.CheckConnection();
			if (!connection.IsSuccess)
			{
                IsRefreshing = false;
				await dialogService.ShowMessage("Error", connection.Message);
				return;
			}

			var mainViewModel = MainViewModel.GetInstance();

            var response = await apiService.Delete(
				"http://productszuluapi.azurewebsites.net",
				"/api",
				"/Categories",
				mainViewModel.Token.TokenType,
				mainViewModel.Token.AccessToken,
				category);

			if (!response.IsSuccess)
			{
				IsRefreshing = false;
				await dialogService.ShowMessage(
					"Error",
					response.Message);
				return;
			}

            categories.Remove(category);
			Categories = new ObservableCollection<Category>(
				categories.OrderBy(c => c.Description));

            IsRefreshing = false;
		}

		async void LoadCategories()
        {
            IsRefreshing = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage(
                    "Error",
                    connection.Message);
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();

            var response = await apiService.GetList<Category>(
                "http://productszuluapi.azurewebsites.net",
                "/api", 
                "/Categories", 
                mainViewModel.Token.TokenType, 
                mainViewModel.Token.AccessToken);

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            categories = (List<Category>)response.Result;
            Search();
            IsRefreshing = false;
        }
        #endregion

        #region Commands
        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(Search);
            }
        }

        void Search()
        {
            IsRefreshing = true;

            if (string.IsNullOrEmpty(Filter))
            {
                Categories = new ObservableCollection<Category>(
                    categories.OrderBy(c => c.Description));
            }
            else
            {
                Categories = new ObservableCollection<Category>(categories
                    .Where(c => c.Description.ToLower().Contains(Filter.ToLower()))
                    .OrderBy(c => c.Description));
            }

            IsRefreshing = false;
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadCategories);
            }
        }
        #endregion
    }
}
