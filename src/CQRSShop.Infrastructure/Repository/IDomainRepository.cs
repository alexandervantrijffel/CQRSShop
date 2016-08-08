using System;
using System.Collections.Generic;

namespace CQRSShop.Infrastructure.Repository
{
    public interface IDomainRepository
    {
        IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        TResult GetById<TResult>(Guid id) where TResult : IAggregate, new();
		int? GetLastEventNumber<T>(Guid id) where T : IAggregate, new();
	}
}