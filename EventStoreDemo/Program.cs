// See https://aka.ms/new-console-template for more information
using EventStore.Client;
using EventStoreDemo;
using Newtonsoft.Json;
using System.Text;

static string StreamId(Guid id)
{
    return $"BanckAccount{id}";
}

//GCRP

var settings = EventStoreClientSettings
    .Create("esdb://localhost:2113?tls=false&tlsVerifyCert=false&keepAliveTimeout=10000&keepAliveInterval=10000");

var client = new EventStoreClient(settings);

var aggregateId = Guid.NewGuid();

List<IEvent> eventsToRun = new List<IEvent>
{
    new AccountCreatedEvent(aggregateId, "Bhavana Shrotri"),
    new FundsDepositedEvent(aggregateId, 100000),
    new FundsWithdrawedEvent(aggregateId, 50000)
};

foreach (var item in eventsToRun)
{
    var jsonString = JsonConvert.SerializeObject(item);
    var jsonPayload = Encoding.UTF8.GetBytes(jsonString);
    var eventStoreDataType = new EventData(Uuid.NewUuid(), item.GetType().Name, jsonPayload, null);
    await client.AppendToStreamAsync(StreamId(aggregateId), StreamState.Any, new[] { eventStoreDataType });
}

var result = client.ReadStreamAsync(
    Direction.Forwards,
    "some-stream",
    EventStore.ClientAPI.StreamPosition.Start);

Console.WriteLine(result);

Console.ReadLine();