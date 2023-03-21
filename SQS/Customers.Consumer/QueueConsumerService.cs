using Amazon.SQS;
using Amazon.SQS.Model;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
  private readonly IAmazonSQS _sqs;
  private readonly IOptions<QueueSettings> _queueSettings;

  public QueueConsumerService(IAmazonSQS sqs, IOptions queueSettings)
  {
    _sqs = sqs;
    _queueSettings = queueSettings;
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
        switch (messageType)
        {
          case nameof(CustomerCreated):
            var created = JsonSerializer.Deserialize<CustomerCreated>(message.Body);
            // do shit with created here.
            break;
          case nameof(CustomerUpdated):
          // var created = JsonSerializer.Deserialize<CustomerUpdated>();
            break;
          case nameof(CustomerDeleted):
          //  var created = JsonSerializer.Deserialize<CustomerDeleted>();
            break;
        }
        await _sqs.DeleteMessageAsync(queUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
      }
      await Task.Delay(1000, stoppingToken);
    }
  }

}