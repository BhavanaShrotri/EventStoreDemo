namespace EventStoreDemo
{
    public interface IEvent
    {
        public int AccountNumber { get; }
    }

    public class AccountCreatedEvent : IEvent
    {
        public int AccountNumber { get; private set; }
        public string Name { get; private set; }

        public AccountCreatedEvent(int accountNumber, string name)
        {
            AccountNumber = accountNumber;
            Name = name;
        }
    }

    public class FundsDepositedEvent : IEvent
    {
        public int AccountNumber { get; private set; }
        public double Amount { get; private set; }

        public FundsDepositedEvent(int accountNumber, double amount)
        {
            AccountNumber = accountNumber;
            Amount = amount;
        }
    }

    public class FundsWithdrawedEvent : IEvent
    {
        public int AccountNumber { get; private set; }
        public double Amount { get; private set; }

        public FundsWithdrawedEvent(int accountNumber, double amount)
        {
            AccountNumber = accountNumber;
            Amount = amount;
        }
    }
}
