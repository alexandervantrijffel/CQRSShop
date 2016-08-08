using System;
using CQRSShop.Contracts.Commands;
using CQRSShop.Contracts.Types;
using CQRSShop.Domain.Exceptions;
using Shouldly;
using Xunit;

namespace IntegrationTests
{
	public class BasketTest : IClassFixture<ApplicationFixture>
	{
		private readonly ApplicationFixture _app;

		public BasketTest(ApplicationFixture app)
		{
			_app = app;
		}

		[Fact]
		public void When_checkout_with_no_items_in_basket_throws_noitemsinbasket()
		{
			var basketId = Guid.NewGuid();
			var customerId = Guid.NewGuid();
			_app.Application.ExecuteCommand(new CreateCustomer(customerId, "Lx Test"));
			_app.Application.ExecuteCommand(new CreateBasket(basketId, customerId));
			Should.Throw<NoItemsInBaskedException>( () => _app.Application.ExecuteCommand(new CheckoutBasket(basketId, new Address("Alohaplein 1"))));
		}

		[Fact]
		public void MakePreferredTest()
		{
			var customerId = Guid.NewGuid();
			_app.Application.ExecuteCommand(new CreateCustomer(customerId, "Lx Test"));
			_app.Application.ExecuteCommand(new MarkCustomerAsPreferred(customerId, 5));
		}
	}
}