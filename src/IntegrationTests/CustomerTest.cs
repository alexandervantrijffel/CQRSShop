using System;
using CQRSShop.Contracts.Commands;
using Xunit;

namespace IntegrationTests
{
	public class CustomerTest : IClassFixture<ApplicationFixture>
	{
		private readonly ApplicationFixture _app;

		public CustomerTest(ApplicationFixture app)
		{
			_app = app;
		}

		[Fact]
		public void Make_preferred_should_succeed()
		{
			var customerId = Guid.NewGuid();
			_app.Application.ExecuteCommand(new CreateCustomer(customerId, "Lx Test"));
			_app.Application.ExecuteCommand(new MarkCustomerAsPreferred(customerId, 5));
		}
	}
}
