using System;
using System.Text;

namespace CQRSShop.Infrastructure.Repository
{
	public class StreamName
	{
		public string Name { get; private set; }

		public StreamName(Type aggregateType, Guid id)
		{
			Name = $"{aggregateType.Name}-{id}";
		}
	}
}
