using CQRSShop.Contracts.Commands;
using CQRSShop.Domain.Aggregates;
using CQRSShop.Domain.Exceptions;
using CQRSShop.Infrastructure;
using CQRSShop.Infrastructure.Exceptions;
using CQRSShop.Infrastructure.Repository;

namespace CQRSShop.Domain.CommandHandlers
{
    internal class ProductCommandHandler : 
        IHandle<CreateProduct>
    {
        private readonly IDomainRepository _domainRepository;

        public ProductCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public IAggregate Handle(CreateProduct command)
        {
	        if (_domainRepository.GetLastEventNumber<Product>(command.Id) != null)
                throw new ProductAlreadyExistsException(command.Id);
            return Product.Create(command.Id, command.Name, command.Price);
        }
    }
}