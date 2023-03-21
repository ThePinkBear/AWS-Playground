using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
{
  public Task<Unit> Handle(CustomerCreated request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}