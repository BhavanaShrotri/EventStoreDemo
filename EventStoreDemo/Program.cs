// See https://aka.ms/new-console-template for more information
using EventStore.Client;
using EventStoreDemo;
using Newtonsoft.Json;
using System.Text;

static string StreamId(int accountNumber)
{
    return $"BanckAccount : {accountNumber}";
}

//GRPC

var settings = EventStoreClientSettings
    .Create("esdb://localhost:2113?tls=false&tlsVerifyCert=false&keepAliveTimeout=10000&keepAliveInterval=10000");

var client = new EventStoreClient(settings);



int key;
do
{
    Console.WriteLine("1 : Create Account");
    Console.WriteLine("2 : Deposit Funds");
    Console.WriteLine("3 : Withdrw Funds");
    Console.WriteLine("4 : Get Account Details");
    Console.WriteLine("5 : Exit ");
    var val = Console.ReadLine();
    key = Convert.ToInt32(val);

    switch (key)
    {
        case 1:
            await CreateAccount();
            break;
        case 2:
            await DepositFunds();
            break;
        case 3:
            await WithdrawFunds();
            break;
        case 4:
            Console.WriteLine("Enter Account Number :");
            var accountNumber = Console.ReadLine();
            var accountDetails = await GetAccountDetails(Convert.ToInt32(accountNumber));
            Console.WriteLine($"Name : {accountDetails.Name}");
            Console.WriteLine($"Balance : {accountDetails.CurrentBalance}");
            break;
        case 5: break;
    }
} while (key != 5);

async Task CreateAccount()
{
    Console.WriteLine("Enter Account Number :");
    var accountNumber = Console.ReadLine();
    Console.WriteLine("Enter Name : ");
    var name = Console.ReadLine();
    await AppendEventToString(new AccountCreatedEvent(Convert.ToInt32(accountNumber), name),
        StreamId(Convert.ToInt32(accountNumber)));
}

async Task DepositFunds()
{
    Console.WriteLine("Enter Account Number :");
    var accountNumber = Console.ReadLine();
    Console.WriteLine("Enter Ammount : ");
    var ammount = Console.ReadLine();
    await AppendEventToString(
        new FundsDepositedEvent(Convert.ToInt32(accountNumber), Convert.ToInt32(ammount)),
        StreamId(Convert.ToInt32(accountNumber)));
}

async Task WithdrawFunds()
{
    Console.WriteLine("Enter Account Number :");
    var accountNumber = Console.ReadLine();
    Console.WriteLine("Enter Ammount : ");
    var ammount = Console.ReadLine();
    await AppendEventToString(
        new FundsWithdrawedEvent(Convert.ToInt32(accountNumber), Convert.ToInt32(ammount)),
        StreamId(Convert.ToInt32(accountNumber)));
}

async Task<BankAccount> GetAccountDetails(int accountNumber)
{
    var results = client.ReadStreamAsync(
    Direction.Forwards,
    StreamId(accountNumber),
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
        else if (eve.Event.EventType == "FundsWithdrawedEvent")
        {
            var state = JsonConvert.DeserializeObject<FundsWithdrawedEvent>(jsonData);
            bankAccount.Apply(state);
        }
    }

    return bankAccount;
}

async Task AppendEventToString(IEvent eve, string streamName)
{
    var jsonString = JsonConvert.SerializeObject(eve);
    var jsonPayload = Encoding.UTF8.GetBytes(jsonString);
    var eventStoreDataType = new EventData(Uuid.NewUuid(), eve.GetType().Name, jsonPayload, null);
    await client.AppendToStreamAsync(streamName, StreamState.Any, new[] { eventStoreDataType });
}

//var aggregateId = 1; ;

//List<IEvent> eventsToRun = new List<IEvent>
//{
//    new AccountCreatedEvent(aggregateId, "Bhavana Shrotri"),
//    new FundsDepositedEvent(aggregateId, 100000),
//    new FundsWithdrawedEvent(aggregateId, 50000),
//    new FundsDepositedEvent(aggregateId, 1000),
//    new FundsWithdrawedEvent(aggregateId, 20000),
//};

//foreach (var item in eventsToRun)
//{
//    var jsonString = JsonConvert.SerializeObject(item);
//    var jsonPayload = Encoding.UTF8.GetBytes(jsonString);
//    var eventStoreDataType = new EventData(Uuid.NewUuid(), item.GetType().Name, jsonPayload, null);
//    await client.AppendToStreamAsync(StreamId(aggregateId), StreamState.Any, new[] { eventStoreDataType });
//}

//var results = client.ReadStreamAsync(
//    Direction.Forwards,
//    StreamId(aggregateId),
//    StreamPosition.Start);


//var ResultData = await results.ToListAsync();

//BankAccount bankAccount = new BankAccount();

//foreach (var eve in ResultData)
//{
//    var jsonData = Encoding.UTF8.GetString(eve.Event.Data.ToArray());

//    if (eve.Event.EventType == "AccountCreatedEvent")
//    {
//        var state = JsonConvert.DeserializeObject<AccountCreatedEvent>(jsonData);
//        bankAccount.Apply(state);
//    }
//    else if (eve.Event.EventType == "FundsDepositedEvent")
//    {
//        var state = JsonConvert.DeserializeObject<FundsDepositedEvent>(jsonData);
//        bankAccount.Apply(state);
//    }
//    else if (eve.Event.EventType == "FundsWithdrawedEvent")
//    {
//        var state = JsonConvert.DeserializeObject<FundsWithdrawedEvent>(jsonData);
//        bankAccount.Apply(state);
//    }
//}

//Console.WriteLine($"Current Balance : {bankAccount.CurrentBalance}");

Console.ReadLine();