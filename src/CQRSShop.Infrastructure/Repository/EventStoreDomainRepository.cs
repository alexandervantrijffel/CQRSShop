using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSShop.Infrastructure.Exceptions;
using EventStore.ClientAPI;

namespace CQRSShop.Infrastructure.Repository
{
    public class EventStoreDomainRepository : DomainRepositoryBase
    {
        private IEventStoreConnection _connection;

        public EventStoreDomainRepository(IEventStoreConnection connection)
        {
            _connection = connection;
        }

		public override int? GetLastEventNumber<T>(Guid id)
		{
			var lastEvent = _connection.ReadEventAsync(new StreamName(typeof(T), id).Name, -1, false).Result;
			return lastEvent?.Event?.OriginalEventNumber;
		}

		public override IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate)
        {
            var events = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, events);
            var eventData = events.Select(CreateEventData);
            var streamName = new StreamName(aggregate.GetType(), aggregate.Id).Name;
            _connection.AppendToStreamAsync(streamName, expectedVersion, eventData).Wait();
            return events;
        }

        public override TResult GetById<TResult>(Guid id)
        {
	        var lastEventNumber = GetLastEventNumber<TResult>(id);
	        if (lastEventNumber == null)
				throw new AggregateNotFoundException($"Could not found aggregate of type {typeof(TResult)} and id {id}");
			var aggregate = new TResult();
	        var streamName = new StreamName(typeof(TResult), id).Name;
	        new Replayer(_connection).ReadStreamAsync(streamName, e => BuildAggregate(aggregate, e)).Wait();
			return aggregate;
        }

	    public EventData CreateEventData(object @event)
        {
            var eventHeaders = new Dictionary<string, string>()
            {
                {
                    Replayer.EventClrTypeHeader, @event.GetType().AssemblyQualifiedName
                },
                {
                    "Domain", "Enheter"
                }
            };
            var eventData = new EventData(Guid.NewGuid(), @event.GetType().Name, true, @event.AsJson(), eventHeaders.AsJson());
            return eventData;
        }

    }
}