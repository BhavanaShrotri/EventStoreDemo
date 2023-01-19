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

var results = client.ReadStreamAsync(
    Direction.Forwards,
    StreamId(aggregateId),
    StreamPosition.Start);


var ResultData = await results.ToListAsync();

BankAccount bankAccount = new BankAccount();

foreach (var eve in ResultData)
{
    var jsonData = Encoding.UTF8.GetString(eve.Event.Data.ToArray());

    if (eve.Event.EventType == "AccountCreatedEvent")
    {
        var state = JsonConvert.DeserializeObject<AccountCreatedEvent>(jsonData);
        bankAccount.Apply(state);
    }
    else if (eve.Event.EventType == "FundsDepositedEvent")
    {
        var state = JsonConvert.DeserializeObject<FundsDepositedEvent>(jsonData);
        bankAccount.Apply(state);
    }
    else if(eve.Event.EventType == "FundsWithdrawedEvent")
    {
        var state = JsonConvert.DeserializeObject<FundsWithdrawedEvent>(jsonData);
        bankAccount.Apply(state);
    }
}

Console.WriteLine($"Current Balance : {bankAccount.CurrentBalance}");
Console.ReadLine();