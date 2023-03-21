using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;
public class CustomerUpdatedHandler : IRequestHandler<CustomerUpdated>
{
  public Task<Unit> Handle(CustomerUpdated request, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
