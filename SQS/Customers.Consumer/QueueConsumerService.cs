using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
  private readonly IAmazonSQS _sqs;
  private readonly IOptions<QueueSettings> _queueSettings;
  private readonly IMediator _mediator;

  public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings, IMediator mediator)
  {
    _sqs = sqs;
    _queueSettings = queueSettings;
    _mediator = mediator;
  }
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var queUrlResponse = await _sqs.GetQueueUrlAsync("customers", stoppingToken);

    var receiveMessageRequest = new ReceiveMessageRequest
    {
      QueueUrl = queUrlResponse.QueueUrl,
      AttributeNames = new List<string> { "All" },
      MessageAttributeNames = new List<string> { "All" },
      MaxNumberOfMessages = 1
    };

    while (!stoppingToken.IsCancellationRequested)
    {
      var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);
      foreach (var message in response.Messages)
      {
        var messageType = message.MessageAttributes["MessageType"];
        


        await _sqs.DeleteMessageAsync(queUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
      }
      await Task.Delay(1000, stoppingToken);
    }
  }

}