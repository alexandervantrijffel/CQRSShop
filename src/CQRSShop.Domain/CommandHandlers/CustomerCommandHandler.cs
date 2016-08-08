using CQRSShop.Contracts.Commands;
using CQRSShop.Domain.Aggregates;
using CQRSShop.Domain.Exceptions;
using CQRSShop.Infrastructure;
using CQRSShop.Infrastructure.Repository;

namespace CQRSShop.Domain.CommandHandlers
{
	internal class CustomerCommandHandler :
		IHandle<CreateCustomer>,
		IHandle<MarkCustomerAsPreferred>
	{
		private readonly IDomainRepository _domainRepository;

		public CustomerCommandHandler(IDomainRepository domainRepository)
		{
			_domainRepository = domainRepository;
		}

		public IAggregate Handle(CreateCustomer command)
		{
			if (_domainRepository.GetLastEventNumber<Customer>(command.Id) == null)
				return Customer.Create(command.Id, command.Name);
			throw new CustomerAlreadyExistsException(command.Id);
		}

		public IAggregate Handle(MarkCustomerAsPreferred command)
		{
			var customer = _domainRepository.GetById<Customer>(command.Id);
			customer.MakePreferred(command.Discount);
			return customer;
		}
	}
}