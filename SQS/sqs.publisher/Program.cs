using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;

var sqsClient = new AmazonSQSClient();
/* In here AmazonSQSClient() <= we can specify region if we want. ex: RegionEndPoint.EUWest2 Or a "new AmasonSQSConfig object that gives access to proxy settings, AWS credentials among other things*/

var customer = new CustomerCreated
{
  Id = Guid.NewGuid(),
  Email = "bjorn.noctiluca@appliedtechnology.se",
  FullName = "Björn Noctiluca",
  DateOfBirth = new DateTime(1986,11,11),
  GitHubUsername = "ThePinkBear"
};

var queUrlResponse = await sqsClient.GetQueueUrlAsync("customers"); // This gets the url agnosticly of region etc instead of hardcoding it in.

var sendMessageRequest = new SendMessageRequest
{
  // Here we specify the message properties, contents of body, url etc.
  QueueUrl = queUrlResponse.QueueUrl,
  MessageBody = JsonSerializer.Serialize(customer),
  // For example, I may want to specify attributes, such as of what type is the message:
  MessageAttributes = new Dictionary<string, MessageAttributeValue>
  {
    {
      "MessageType", new MessageAttributeValue
      {
        DataType = "String",
        StringValue = nameof(CustomerCreated)
      }
    }
  },
  // I can also add delays to the message or ovveride default behaviours here
};

var response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.WriteLine("Hello World!");