using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace CQRSShop.Infrastructure.Repository
{
	public class Replayer
	{
		public const string EventClrTypeHeader = "EventClrTypeName";
		private readonly IEventStoreConnection _connection;
		private int _startEvent;

		public Replayer(IEventStoreConnection connection)
		{
			_connection = connection;
		}

		public int? GetLastEventNumber(string streamName)
		{
			var lastEvent = _connection.ReadEventAsync(streamName, -1, false).Result;
			return lastEvent?.Event?.OriginalEventNumber;
		}

		/// <summary>
		/// Replays all events for the given stream name, from startEvent to end.
		/// </summary>
		/// <param name="processor">in events, out boolean for continue</param>
		public async Task<bool> ReadStreamAsync(string streamName, Action<IEnumerable<IEvent>> processor, int startEvent = 0)
		{
			var lastEventNumber = GetLastEventNumber(streamName) ?? 0;
			_startEvent = startEvent;
			do
			{
				processor(await ReadStreamAsync(streamName));
			} while (_startEvent <= lastEventNumber);
			return true;
		}

		private async Task<IEnumerable<IEvent>> ReadStreamAsync(string streamName)
		{
			var pageSize = 4096; // maximum page size that is supported by GetEventStore

			var events = await _connection.ReadStreamEventsForwardAsync(streamName, _startEvent, pageSize, false);
			_startEvent = events.NextEventNumber;
			var deserializedEvents = events.Events.Select(e =>
			{
				var metadata =
					JsonExtensions.ToObject<Dictionary<string, string>>(e.OriginalEvent.Metadata, typeof(Dictionary<string, string>).AssemblyQualifiedName);
				return JsonExtensions.ToObject<IEvent>(e.OriginalEvent.Data, metadata[EventClrTypeHeader]);
			});
			return deserializedEvents;
		}
	}
}