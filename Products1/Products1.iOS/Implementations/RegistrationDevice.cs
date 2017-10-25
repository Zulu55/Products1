using Xamarin.Forms;

[assembly: Dependency(typeof(Products1.iOS.Implementations.RegistrationDevice))]

namespace Products1.iOS.Implementations
{
    using Interfaces;
    using UIKit;
    using Foundation;

    public class RegistrationDevice: IRegisterDevice
    {
        #region Methods
        public void RegisterDevice()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | 
                        UIUserNotificationType.Badge | 
                        UIUserNotificationType.Sound,
                       new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(
                    pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = 
                    UIRemoteNotificationType.Alert |
                    UIRemoteNotificationType.Badge | 
                    UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.
                             RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }
        #endregion
    }
}