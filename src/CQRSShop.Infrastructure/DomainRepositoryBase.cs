using System;
using System.Collections.Generic;
using CQRSShop.Infrastructure.Repository;

namespace CQRSShop.Infrastructure
{
    public abstract class DomainRepositoryBase : IDomainRepository
    {
        public abstract IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        public abstract TResult GetById<TResult>(Guid id) where TResult : IAggregate, new();
		public abstract int? GetLastEventNumber<T>(Guid id) where T : IAggregate, new();
		
		protected int CalculateExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            var expectedVersion = aggregate.Version - events.Count;
            return expectedVersion;
        }
		
		protected TResult BuildAggregate<TResult>(IEnumerable<IEvent> events) where TResult : IAggregate, new()
        {
            var result = new TResult();
            foreach (var @event in events)
            {
                result.ApplyEvent(@event);
            }
            return result;
        }
    }
}