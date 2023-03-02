using SqsConsumer;
using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();
var sqsClient = new AmazonSQSClient();

var queUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var receiveMessageRequest = new ReceiveMessageRequest
{
  QueueUrl = queUrlResponse.QueueUrl, // This is the only parameter required, the URL we get with the, in this case, customers.

  // SQS by default tries to be efficient and omits Attributes and system attributes on received messages, if we want to see them we need to specify that here.
  AttributeNames = new List<string>{ "All"},  // "All" gives all the attributes but if we know what we want we can just give that name
  MessageAttributeNames = new List<string>{ "All" } // Same goes here
};

while(!cts.IsCancellationRequested)
{
  var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);

  foreach (var message in response.Messages)
  {
    Console.WriteLine($"Message Id: {message.MessageId}");
    Console.WriteLine($"Message Body: {message.Body}");

    // This following line is required for the message to actually be consumed and not just read and left in the queue
    await sqsClient.DeleteMessageAsync(queUrlResponse.QueueUrl, message.ReceiptHandle);
  }

  await Task.Delay(3000);
}