using System;
using EventStore.ClientAPI;
using Xunit;

namespace IntegrationTests
{
	public class ReplayTest : IClassFixture<ApplicationFixture>
	{
		private readonly ApplicationFixture _app;

		public ReplayTest(ApplicationFixture app)
		{
			_app = app;
		}

		[Fact]
		public void Replay()
		{
			//_app.Connecton.SubscribeToStreamFrom()
			//_latestPosition = ;
			//_connection = EventStoreConnectionWrapper.Connect();
			//_connection.Connected +=
			//	(sender, args) => _connection.SubscribeToAllFrom(Position.Start, false, HandleEvent);
			//Console.WriteLine("Indexing service started");
		}
	}


}