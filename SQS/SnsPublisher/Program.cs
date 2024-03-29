﻿using SnsPublisher;
using System.Collections.Generic;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Text.Json;


var customer = new CustomerCreated
{
  Id = Guid.NewGuid(),
  Email = "bjorn.noctiluca@outlook.com",
  FullName = "Bear Noctiluca",
  DateOfBirth = new DateTime(1986, 11, 11),
  GitHubUsername = "thepinkbear"
};

var snsClient = new AmazonSimpleNotificationServiceClient();

var topicArnResponse = await snsClient.FindTopicAsync("customers");

var publishRequest = new PublishRequest
{
  TopicArn = topicArnResponse.TopicArn,
  Message = JsonSerializer.Serialize(customer),
  MessageAttributes = new Dictionary<string, MessageAttributeValue>
  {
    {
      "MessageType", new MessageAttributeValue
      {
        DataType = "String",
        StringValue = nameof(CustomerCreated)
      }
    }
  }
};

var response = await snsClient.PublishAsync(publishRequest);