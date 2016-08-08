using CQRSShop.Contracts.Commands;
using CQRSShop.Domain.Aggregates;
using CQRSShop.Domain.Exceptions;
using CQRSShop.Infrastructure;
using CQRSShop.Infrastructure.Exceptions;
using CQRSShop.Infrastructure.Repository;

namespace CQRSShop.Domain.CommandHandlers
{
	internal class BasketCommandHandler :
		IHandle<CreateBasket>,
		IHandle<AddItemToBasket>,
		IHandle<ProceedToCheckout>,
		IHandle<CheckoutBasket>,
		IHandle<MakePayment>
	{
		private readonly IDomainRepository _domainRepository;

		public BasketCommandHandler(IDomainRepository domainRepository)
		{
			_domainRepository = domainRepository;
		}

		public IAggregate Handle(CreateBasket command)
		{
			if (_domainRepository.GetLastEventNumber<Customer>(command.Id) != null)
				throw new BasketAlreadExistsException(command.Id);
				
			var customer = _domainRepository.GetById<Customer>(command.CustomerId);
			return Basket.Create(command.Id, customer);
		}

		public IAggregate Handle(AddItemToBasket command)
		{
			var basket = _domainRepository.GetById<Basket>(command.Id);
			var product = _domainRepository.GetById<Product>(command.ProductId);
			basket.AddItem(product, command.Quantity);
			return basket;
		}

		public IAggregate Handle(ProceedToCheckout command)
		{
			var basket = _domainRepository.GetById<Basket>(command.Id);
			basket.ProceedToCheckout();
			return basket;
		}

		public IAggregate Handle(CheckoutBasket command)
		{
			var basket = _domainRepository.GetById<Basket>(command.Id);
			basket.Checkout(command.ShippingAddress);
			return basket;
		}

		public IAggregate Handle(MakePayment command)
		{
			var basket = _domainRepository.GetById<Basket>(command.Id);
			var order = basket.MakePayment(command.Payment);
			return order;
		}
	}
}