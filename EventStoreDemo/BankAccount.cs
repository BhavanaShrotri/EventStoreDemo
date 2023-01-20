
namespace EventStoreDemo
{
    public class BankAccount
    {
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public double CurrentBalance { get; set; }
        
        public void Apply(AccountCreatedEvent @event)
        {
            AccountNumber = @event.AccountNumber;
            Name = @event.Name;
            CurrentBalance = 0;
        }

        public void Apply(FundsDepositedEvent @event)
        {
            CurrentBalance = CurrentBalance + @event.Amount;
        }

        public void Apply(FundsWithdrawedEvent @event)
        {
            CurrentBalance = CurrentBalance - @event.Amount;
        }
    }
}
