using System;
using System.Collections.Generic;
using System.Linq;
using CQRSShop.Contracts.Events;

namespace CQRSShop.Service.Custom
{
	public class EventDispatcher
	{
		public void Dispatch(dynamic @event, Denormalizer replayer)
		{
			if (!ResolveSupportedTypes(replayer).Any(t => t == @event.GetType()))
				throw new Exception($"Denormalizer {replayer.GetType().Name} does not support event {@event.GetType().Name}");
			replayer.Handle(@event);
		}

		private IEnumerable<Type> ResolveSupportedTypes(Denormalizer denormalizer)
		{
			foreach (var method in denormalizer.GetType().GetMethods().Where(m => m.Name.StartsWith("Handle")))
			{
				foreach (var p in method.GetParameters())
				{
					yield return p.ParameterType;
				}
			}
		}
	}

	public class Denormalizer
	{
		public void Handle(CustomerCreated @event)
		{

		}

		public void Handle(OrderShipped @event)
		{

		}

		public void Handle(BasketCreated @event)
		{

		}
	}
}
