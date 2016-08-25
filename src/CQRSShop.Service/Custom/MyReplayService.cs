using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRSShop.Contracts.Events;
using CQRSShop.Infrastructure.Repository;
using CQRSShop.Service.Custom;
using CQRSShop.Service.Documents;
using EventStore.ClientAPI;
using Replayer = CQRSShop.Infrastructure.Repository.Replayer;

namespace CQRSShop.Service
{
	public class MyReplayService
	{
		private Position? _latestPosition;
		private IEventStoreConnection _connection;

		public void Start()
		{
			ConnectToEventstore();
		}
		
		public void Stop()
		{
			
		}

		private void ConnectToEventstore()
		{
			var stream = new StreamName(typeof(Customer), Guid.NewGuid());
			_latestPosition = Position.Start;
			_connection = EventStoreConnectionWrapper.Connect();
			_connection.Connected += (sender, args) =>
			{
				_connection.SubscribeToAllFrom(_latestPosition, false, HandleEvent);
			};

			Console.WriteLine("Indexing service started");
		}

		private void HandleEvent(EventStoreCatchUpSubscription arg1, ResolvedEvent arg2)
		{

			var @event = EventSerialization.DeserializeEvent(arg2.OriginalEvent);
			if (@event != null)
			{
				dynamic casted = Convert.ChangeType(@event, @event.GetType());
				var denormalizer = new Denormalizer();
				new EventDispatcher().Dispatch(casted, denormalizer);
			}
			_latestPosition = arg2.OriginalPosition;
		}
	}
}
