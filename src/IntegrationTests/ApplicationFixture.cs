using System;
using CQRSShop.Domain;
using CQRSShop.Infrastructure;
using CQRSShop.Infrastructure.Repository;
using CQRSShop.Web;
using EventStore.ClientAPI;

namespace IntegrationTests
{
	public class ApplicationFixture : IDisposable
	{
		private DomainEntry _application;
		private IEventStoreConnection _connection;
		public DomainEntry Application => _application = _application ?? CreateApplication();

		private DomainEntry CreateApplication()
		{
			_connection = Configuration.CreateConnection();
			var domainRepository = new EventStoreDomainRepository(_connection);
			return new DomainEntry(domainRepository);
		}
		public void Dispose()
		{
			_connection?.Close();
		}
	}
}