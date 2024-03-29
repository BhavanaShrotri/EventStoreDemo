﻿// See https://aka.ms/new-console-template for more information
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
    Console.WriteLine("2 : Funds Transactions");
    Console.WriteLine("3 : Get Account Details");
    Console.WriteLine("4 : Exit ");
    var val = Console.ReadLine();
    key = Convert.ToInt32(val);

    Console.Write("Enter Account Number :");
    var accountNumber = Convert.ToInt32(Console.ReadLine());

    switch (key)
    {
        case 1:
            Console.Write("Enter Name : ");
            var name = Console.ReadLine();
            await CreateAccount(accountNumber, name);
            break;
        case 2:
            Console.WriteLine("1 : Deposit Funds");
            Console.WriteLine("2 : Withdrw Funds");
            var choice = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Amount : ");
            var amount = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    await DepositFunds(accountNumber, amount);
                    break;
                case 2:
                    await WithdrawFunds(accountNumber, amount);
                    break;
            }
            break;
        case 3:
            var accountDetails = await GetAccountDetails(accountNumber);
            Console.WriteLine($"Name : {accountDetails.Name}");
            Console.WriteLine($"Balance : {accountDetails.CurrentBalance}");
            break;
        case 4: break;
    }
} while (key != 4);

async Task CreateAccount(int accountNumber, string name)
{
    await AppendEventToString(new AccountCreatedEvent(accountNumber, name),
        StreamId(accountNumber));
}

async Task DepositFunds(int accountNumber, int amount)
{
    if (await AccountExists(accountNumber))
    {
        await AppendEventToString(
            new FundsDepositedEvent(accountNumber, amount),
            StreamId(accountNumber));
    }
}

async Task WithdrawFunds(int accountNumber, int amount)
{
    if (await AccountExists(accountNumber))
    {
        await AppendEventToString(
            new FundsWithdrawedEvent(accountNumber, amount),
            StreamId(accountNumber));
    }
}

async Task<bool> AccountExists(int accountNumber)
{
    var result = client.ReadStreamAsync(
    Direction.Forwards,
    StreamId(accountNumber), StreamPosition.Start, 1);

    if (await result.ReadState == ReadState.StreamNotFound)
    {
        Console.Write("Account Not exists");
        return false;
    }
    return true;
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

//Console.Write($"Current Balance : {bankAccount.CurrentBalance}");

Console.ReadLine();