using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project.Blockchain;

public class TestChain : BlockchainBase
{
    public override string Name { get; } = "Test Chain";
    public override int Difficulty { get; protected set; } = 5;
    protected override decimal MiningReward { get; set; } = 100;
    private void Initiate()
    {
        Chain.Add(CreateGenesisBlock());
    }
    public TestChain()
    {
        Initiate();
    }
    protected override List<ITransact> PendingTransactions { get;  } = new();
    public override IReadOnlyList<ITransact> GetPendingTransactions() => PendingTransactions.AsReadOnly();
    protected override List<IBlock> Chain { get;  } = new();
    public override IReadOnlyList<IBlock> GetChain() => Chain.AsReadOnly();
    public override IBlock GetLatestBlock() => Chain.AsReadOnly().LastOrDefault() ?? throw new InvalidOperationException("Blockchain is empty.");
    protected override void ClearPendingTransactions()
    {
        PendingTransactions.Clear();
    }

    //Test Chain does not require transaction signing
    protected override bool VerifyTransaction(ITransact transaction)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(transaction);
            if (transaction.FromAddress == "System")
            {
                return true; // System transactions (like mining rewards) do not require balance checks
            }
            if (string.IsNullOrEmpty(transaction.FromAddress) || string.IsNullOrEmpty(transaction.ToAddress))
                throw new ArgumentException("Transaction must have valid From and To addresses.");
            if (transaction.Amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transaction.Amount));
            var pendingTx = PendingTransactions.FindAll(t => t.FromAddress == transaction.FromAddress);
            var totalPendingAmount = pendingTx
                .Sum(t => t.Amount);
            if (totalPendingAmount + transaction.Amount > GetBalance(transaction.FromAddress))
                throw new InvalidOperationException("Insufficient balance for the transaction.");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error verifying transaction: {e.Message}");
            return false;
        }
    }

    public override void AddTransaction(ITransact transaction)
    {
        try
        {
            if(!VerifyTransaction(transaction))
            {
                throw new InvalidOperationException("Transaction verification failed.");
            }
            PendingTransactions.Add(transaction);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding transaction: {e.Message}");
            throw;
        }
    }

}