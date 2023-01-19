namespace EventStoreDemo
{
    public class IEvent
    {
        public Guid Id { get; }
    }

    public class AccountCreatedEvent : IEvent
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public AccountCreatedEvent(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class FundsDepositedEvent : IEvent
    {
        public Guid Id { get; private set; }
        public Decimal Amount { get; private set; }

       public FundsDepositedEvent(Guid id, Decimal amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    public class FundsWithdrawedEvent : IEvent
    {
        public Guid Id { get; private set; }
        public Decimal Amount { get; private set; }

        public FundsWithdrawedEvent(Guid id, Decimal amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}
