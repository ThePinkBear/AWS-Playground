using Customers.Api.Contracts.Data;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Net;

namespace Customers.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
  private readonly IAmazonDynamoDB _dynamoDb;
  private readonly string _tableName = "customers";

  public CustomerRepository(IAmazonDynamoDB dynamoDb)
  {
    _dynamoDb = dynamoDb;
  }

  public async Task<bool> CreateAsync(CustomerDto customer)
  {
    customer.UpdatedAt = DateTime.UtcNow;
    var customerAsJson = JsonSerializer.Serialize(customer);
    var customerASAttributes = Document.FromJson(customerAsJson).ToAttributeMap();
    var createItemRequest = new PutItemRequest
    {
      TableName = _tableName,
      Item = customerASAttributes
    };
    var response = await _dynamoDb.PutItemAsync(createItemRequest);
    return response.HttpStatusCode ==  HttpStatusCode.OK;
  }

  public async Task<CustomerDto?> GetAsync(Guid id)
  {
    var getItemRequest = new GetItemRequest
    {
      TableName = _tableName,
      Key = new Dictionary<string, AttributeValue>
      {       
          { "pk", new AttributeValue{ S = id.ToString() } },
          { "sk", new AttributeValue{ S = id.ToString() } }       
      }
    };
    var response = await _dynamoDb.GetItemAsync(getItemRequest);
    if (response.Item.Count == 0)
    {
      return null;
    }
    var itemAsDocument = Document.FromAttributeMap(response.Item);
    return JsonSerializer.Deserialize<CustomerDto>(itemAsDocument.ToJson());
  }

  public async Task<IEnumerable<CustomerDto>> GetAllAsync()
  {
    throw new NotImplementedException();
  }

  public async Task<bool> UpdateAsync(CustomerDto customer)
  {
    customer.UpdatedAt = DateTime.UtcNow;
    var customerAsJson = JsonSerializer.Serialize(customer);
    var customerASAttributes = Document.FromJson(customerAsJson).ToAttributeMap();
    var updateItemRequest = new PutItemRequest
    {
      TableName = _tableName,
      Item = customerASAttributes
    };
    var response = await _dynamoDb.PutItemAsync(updateItemRequest);
    return response.HttpStatusCode ==  HttpStatusCode.OK;
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var deleteItemRequest = new DeleteItemRequest
    {
      TableName = _tableName,
      Key = new Dictionary<string, AttributeValue>
      {       
          { "pk", new AttributeValue{ S = id.ToString() } },
          { "sk", new AttributeValue{ S = id.ToString() } }       
      }
    };
    var response = await _dynamoDb.DeleteItemAsync(deleteItemRequest);
    return response.HttpStatusCode == HttpStatusCode.OK;
  }
}
