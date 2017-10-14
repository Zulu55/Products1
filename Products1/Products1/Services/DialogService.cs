namespace Products1.Services
{
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public class DialogService
    {
		public async Task ShowMessage(string title, string message)
		{
			await Application.Current.MainPage.DisplayAlert(
				title,
				message,
				"Accept");
		}

		public async Task<bool> ShowConfirm(string title, string message)
		{
			return await Application.Current.MainPage.DisplayAlert(
				title,
				message,
				"Yes",
                "No");
		}

        public async Task<string> ShowImageOptions()
		{
			return await Application.Current.MainPage.DisplayActionSheet(
				"Where do you take the image?",
				"Cancel",
				null,
				"From Gallery",
				"From Camera");
		}
	}
}
