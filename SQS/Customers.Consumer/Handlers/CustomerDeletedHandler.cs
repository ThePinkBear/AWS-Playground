using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerDeletedHandler : IRequestHandler<CustomerDeleted>
{
  public Task<Unit> Handle(CustomerDeleted request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
