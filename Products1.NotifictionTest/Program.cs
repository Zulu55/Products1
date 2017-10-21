namespace Products1.NotifictionTest
{
    using Microsoft.Azure.NotificationHubs;
    using System;
    using System.Collections.Generic;

    class Program
    {
        private static NotificationHubClient hub;

        public static void Main(string[] args)
        {
            hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=" +
                "sb://productsclass.servicebus.windows.net/;" +
                "SharedAccessKeyName=DefaultFullSharedAccessSignature;" +
                "SharedAccessKey=H8SNoyeqmGwTUpenZ2DArE9mLhHS3WrAFPL96wpwp6s=", 
                "ProductsClass");

            do
            {
                Console.WriteLine("Type a newe message:");
                var message = Console.ReadLine();
                SendNotificationAsync(message);
                Console.WriteLine("The message was sent...");
            } while (true);
        }

        private static async void SendNotificationAsync(string message)
        {
            var tags = new List<string>();
            tags.Add("UserId:jzluaga55@gmail.com");
            tags.Add("UserId:valery@gmail.com");
            await hub.SendGcmNativeNotificationAsync(
                "{ \"data\" : {\"Message\":\"" + message + "\"}}", tags);
        }

    }
}
