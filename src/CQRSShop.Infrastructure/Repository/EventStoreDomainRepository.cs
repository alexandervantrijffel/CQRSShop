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
        private const string Category = "cqrsshop";

        public EventStoreDomainRepository(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        private string AggregateToStreamName(Type type, Guid id)
        {
            return string.Format("{0}-{1}-{2}", Category, type.Name, id);
        }

		public override int? GetLastEventNumber<T>(Guid id)
		{
			var lastEvent = _connection.ReadEventAsync(AggregateToStreamName(typeof(T), id), -1, false).Result;
			return lastEvent?.Event?.OriginalEventNumber;
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
	        var lastEventNumber = GetLastEventNumber<TResult>(id);
	        if (lastEventNumber == null)
	        {
				throw new AggregateNotFoundException($"Could not found aggregate of type {typeof(TResult)} and id {id}");
			}
			return GetFromStreamAsync<TResult>(id, lastEventNumber).Result;
        }

	    private async Task<TResult> GetFromStreamAsync<TResult>(Guid id, int? lastEventNumber) where TResult:IAggregate,new()
	    {
			var streamName = AggregateToStreamName(typeof(TResult), id);
			var aggregate = new TResult();
			var pageSize = 4096; // maximum page size that is supported by GetEventStore
			var startEvent = 0;

			do
		    {
			    var events = await _connection.ReadStreamEventsForwardAsync(streamName, startEvent, pageSize, false);
			    startEvent = events.NextEventNumber;
			    var deserializedEvents = events.Events.Select(e =>
			    {
				    var metadata =
					    e.OriginalEvent.Metadata.ToObject<Dictionary<string, string>>(
						    typeof(Dictionary<string, string>).AssemblyQualifiedName);
				    return e.OriginalEvent.Data.ToObject<IEvent>(metadata[EventClrTypeHeader]);
			    });
			    BuildAggregate(aggregate, deserializedEvents);
		    } while (startEvent <= lastEventNumber.Value);
		    return aggregate;
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