using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project.Blockchain;

public class Mainchain : BlockchainBase
{
    public override string Name { get; } = "Main Chain";
    protected override List<IBlock> Chain {get; } = new();
    public override IReadOnlyList<IBlock> GetChain() => Chain.AsReadOnly();
    public override int Difficulty { get; protected set; } = 5;
    protected override decimal MiningReward { get; set; } = 50;
    private decimal Fee { get; set; } = 0.25m;
    public override decimal GetFee() => Fee;
    public override decimal GetMiningReward() => MiningReward + Fee;
    protected override List<ITransact> PendingTransactions { get;  } = new();
    public override IReadOnlyList<ITransact> GetPendingTransactions() => PendingTransactions.AsReadOnly();
    protected override Block CreateGenesisBlock()
    {
        // Create the first block in the blockchain with a default previous hash of "0"
        var genesisTransaction = new Transaction("System", "0", 1);
        PendingTransactions.Add(genesisTransaction);
        return new Block("0", PendingTransactions);
    }
    private void Initiate()
    {
        Chain.Add(CreateGenesisBlock());
    }
    public Mainchain()
    {
        Initiate();
    }
    public override IBlock GetLatestBlock() => Chain.AsReadOnly().LastOrDefault() ?? throw new InvalidOperationException("Blockchain is empty.");
    protected override void ClearPendingTransactions()
    {
        PendingTransactions.Clear();
    }
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
            if (!transaction.VerifySignature())
                throw new InvalidOperationException("Transaction signature is invalid.");
            var amountWithFee = transaction.Amount + Fee;
            if (GetBalance(transaction.FromAddress) < amountWithFee)
                throw new InvalidOperationException("Insufficient balance for the transaction.");
            // Get all pending transactions from the same sender
            var pendingTx = PendingTransactions.FindAll(t => t.FromAddress == transaction.FromAddress);
            var totalPendingAmount = pendingTx
                .Sum(t => t.Amount);
            if (totalPendingAmount + amountWithFee > GetBalance(transaction.FromAddress))
                throw new InvalidOperationException("Insufficient balance for the transaction.");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Transaction verification failed: {e.Message}");
            return false;
        }
    }

    public override void AddTransaction(ITransact transaction)
    {
        try
        {
            var verify = VerifyTransaction(transaction);
            if (!verify)
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

    public override void AddTransaction(ITransact[] transacts)
    {
        try
        {
            if (transacts.Any(tx => !VerifyTransaction(tx)))
            {
                throw new InvalidOperationException("Transact verification failed.");
            }
            PendingTransactions.AddRange(transacts);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding transactions: {e.Message}");
            throw;
        }
    }

    public override decimal GetBalance(string address)
    {
        try
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentException("Address cannot be null or empty.", nameof(address));
            return CalculateAddressBalance(address);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting balance for address {address}: {e.Message}");
            throw;
        }
    }

}