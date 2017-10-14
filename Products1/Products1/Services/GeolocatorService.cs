namespace Products1.Services
{
    using System;
    using System.Threading.Tasks;
    using Plugin.Geolocator;

    public class GeolocatorService
    {
        #region Properties
        public double Latitude
        {
            get;
            set;
        }

        public double Longitude
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public async Task GetLocation()
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var location = await locator.GetPositionAsync();
                Latitude = location.Latitude;
                Longitude = location.Longitude;

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        #endregion
    }
}
