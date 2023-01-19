// See https://aka.ms/new-console-template for more information
using EventStore.Client;
using EventStore.ClientAPI;
using EventStoreDemo;
using Newtonsoft.Json;
using System.Net;
using System.Text;

static string StreamId(Guid id)
{
    return $"BanckAccount{id}";
}
var address = IPAddress.Parse("127.0.0.1");

//var settings = ConnectionSettings
//    .Create()
//    .DisableServerCertificateValidation()
//    .DisableTls()
//    .KeepReconnecting()
//    .Build();

//IEventStoreConnection connection = EventStoreConnection.Create(
//        new IPEndPoint(IPAddress.Loopback, 1113));

//await connection.ConnectAsync();

//GCRP

var settings = EventStoreClientSettings
    .Create("esdb://localhost:2113?tls=false&tlsVerifyCert=false&keepAliveTimeout=10000&keepAliveInterval=10000");

var client = new EventStoreClient(settings);

var evt = new
{
    EntityId = Guid.NewGuid().ToString("N"),
    mportantData = "I wrote my first event!"
};
var jsonString = JsonConvert.SerializeObject(evt);

var eventData = new EventStore.Client.EventData(
    Uuid.NewUuid(),
    "TestEvent",
     Encoding.UTF8.GetBytes(jsonString)
);

await client.AppendToStreamAsync(
    "some-stream",
    StreamRevision.None,
     new[] { eventData });

var result = client.ReadStreamAsync(
    Direction.Forwards,
    "some-stream",
    EventStore.ClientAPI.StreamPosition.Start);


//var aggregateId = Guid.NewGuid();

//List<IEvent> eventsToRun = new List<IEvent>
//{
//    new AccountCreatedEvent(aggregateId, "Bhavana Shrotri"),
//    new FundsDepositedEvent(aggregateId, 100000),
//    new FundsWithdrawedEvent(aggregateId, 50000)
//};

//foreach (var item in eventsToRun)
//{
//    var jsonString = JsonConvert.SerializeObject(item);
//    var jsonPayload = Encoding.UTF8.GetBytes(jsonString);
//    var eventStoreDataType = new EventStore.ClientAPI.EventData(Guid.NewGuid(), item.GetType().Name, true, jsonPayload, null);
//    await connection.AppendToStreamAsync(StreamId(aggregateId), ExpectedVersion.Any, eventStoreDataType);
//}

Console.ReadLine();