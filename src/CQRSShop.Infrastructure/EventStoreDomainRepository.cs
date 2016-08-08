using System;
using System.Collections.Generic;
using System.Linq;
using CQRSShop.Infrastructure.Exceptions;
using EventStore.ClientAPI;

namespace CQRSShop.Infrastructure
{
    public class EventStoreDomainRepository : DomainRepositoryBase
    {
        private IEventStoreConnection _connection;
        private const string Category = "cqrsshop";

        public EventStoreDomainRepository(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        private string AggregateToStreamName(Type type, Guid id)
        {
            return string.Format("{0}-{1}-{2}", Category, type.Name, id);
        }

        public override IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate)
        {
            var events = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, events);
            var eventData = events.Select(CreateEventData);
            var streamName = AggregateToStreamName(aggregate.GetType(), aggregate.Id);
            _connection.AppendToStreamAsync(streamName, expectedVersion, eventData).Wait();
            return events;
        }

        public override TResult GetById<TResult>(Guid id)
        {
            var streamName = AggregateToStreamName(typeof(TResult), id);
			// todo paging
            var eventsSlice = _connection.ReadStreamEventsForwardAsync(streamName, 0, 4096, false).Result;
            if (eventsSlice.Status == SliceReadStatus.StreamNotFound)
            {
                throw new AggregateNotFoundException("Could not found aggregate of type " + typeof(TResult) + " and id " + id);
            }
            var deserializedEvents = eventsSlice.Events.Select(e =>
            {
	            var metadata =
		            e.OriginalEvent.Metadata.ToObject<Dictionary<string, string>>(
			            typeof(Dictionary<string, string>).AssemblyQualifiedName);
	            return e.OriginalEvent.Data.ToObject<IEvent>(metadata[EventClrTypeHeader]);
            });
            return BuildAggregate<TResult>(deserializedEvents);
        }

        public EventData CreateEventData(object @event)
        {
            var eventHeaders = new Dictionary<string, string>()
            {
                {
                    EventClrTypeHeader, @event.GetType().AssemblyQualifiedName
                },
                {
                    "Domain", "Enheter"
                }
            };
            var eventData = new EventData(Guid.NewGuid(), @event.GetType().Name, true, @event.AsJson(), eventHeaders.AsJson());
            return eventData;
        }

        public string EventClrTypeHeader = "EventClrTypeName";
    }
}