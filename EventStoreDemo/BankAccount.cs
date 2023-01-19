
namespace EventStoreDemo
{
    public class BankAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Decimal CurrentBalance { get; set; }
        public List<Transaction> Transactions = new List<Transaction>();

        public void Apply(AccountCreatedEvent @event)
        {
            Id = @event.Id;
            Name = @event.Name;
            CurrentBalance = 0;
        }

        public void Apply(FundsDepositedEvent @event)
        {
            var fundsDepositedTransaction = new Transaction { Id = @event.Id , Ammount = @event.Amount};
            Transactions.Add(fundsDepositedTransaction);
            CurrentBalance = CurrentBalance + @event.Amount;
        }

        public void Apply(FundsWithdrawedEvent @event)
        {
            var fundsWithdrawTransaction = new Transaction { Id = @event.Id, Ammount = @event.Amount };
            Transactions.Add(fundsWithdrawTransaction);
            CurrentBalance = CurrentBalance - @event.Amount;
        }
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public Decimal Ammount { get; set; }
    }
}
