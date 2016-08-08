using System.Configuration;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace CQRSShop.Web
{
    public static class Configuration
    {
        private static IEventStoreConnection _connection;
        public static IEventStoreConnection CreateConnection()
        {
            return _connection = _connection ?? Connect();
        }

        private static IEventStoreConnection Connect()
        {
            ConnectionSettings settings =
                ConnectionSettings.Create()
                    .UseConsoleLogger()
                    .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));
            var endPoint = new IPEndPoint(EventStoreIP, EventStorePort);
            var connection = EventStoreConnection.Create(settings, endPoint);
	        connection.ConnectAsync().Wait();
	        return connection;
        }

	    private static IPAddress EventStoreIP
        {
            get
            {
                var hostname = ConfigurationManager.AppSettings["EventStoreHostName"];
                if (string.IsNullOrEmpty(hostname))
                {
                    return IPAddress.Loopback;
                }
                var ipAddresses = Dns.GetHostAddresses(hostname);
                return ipAddresses[0];
            }
        }

	    private static int EventStorePort
        {
            get
            {
                var esPort = ConfigurationManager.AppSettings["EventStorePort"];
                if (string.IsNullOrEmpty(esPort))
                {
                    return 1113;
                }
                return int.Parse(esPort);
            }
        }
    }
}