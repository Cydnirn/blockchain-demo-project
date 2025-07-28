using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project;

public class Blockchain : IBlockchain
{
    public string Name { get; }
    private List<IBlock> Chain {get;  } = new();
    public IReadOnlyList<IBlock> GetChain() => Chain.AsReadOnly();
    public int Difficulty { get; private set; }
    public decimal MiningReward { get; private set; }
    private List<ITransact> PendingTransactions { get;  } = new();
    public IReadOnlyList<ITransact> GetPendingTransactions() => PendingTransactions.AsReadOnly();

    public Blockchain()
    {
        Name = "Wiwok Chain";
        Difficulty = 10;
        MiningReward = 50;
        Chain.Add(CreateGenesisBlock());
    }

    private Block CreateGenesisBlock()
    {
        // Create the first block in the blockchain with a default previous hash of "0"
        var genesisTransaction = new Transaction("System", "0", 1);
        PendingTransactions.Add(genesisTransaction);
        return new Block("0", PendingTransactions);
    }

    public IBlock GetLatestBlock() => GetChain().LastOrDefault() ?? throw new InvalidOperationException("Blockchain is empty.");

    private void ClearPendingTransactions()
    {
        PendingTransactions.Clear();
    }

    public virtual void AddBlock(IBlock block)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        if (!block.ValidBlock(Difficulty)) throw new InvalidOperationException("Invalid block.");

        Chain.Add(block);
        ClearPendingTransactions();
    }

    private bool VerifyTransaction(ITransact transaction)
    {
        try
        {
            if (transaction.FromAddress == "System")
            {
                return true; // System transactions (like mining rewards) do not require balance checks
            }
            ArgumentNullException.ThrowIfNull(transaction);
            if (string.IsNullOrEmpty(transaction.FromAddress) || string.IsNullOrEmpty(transaction.ToAddress))
                throw new ArgumentException("Transaction must have valid From and To addresses.");
            if (transaction.Amount <= 0)
                throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transaction.Amount));
            if (!transaction.VerifySignature())
                throw new InvalidOperationException("Transaction signature is invalid.");
            if (GetBalance(transaction.FromAddress) < transaction.Amount)
                throw new InvalidOperationException("Insufficient balance for the transaction.");

            var pendingTx = PendingTransactions.FindAll(t => t.FromAddress == transaction.FromAddress);
            if(pendingTx.Count < 0)
                throw new InvalidOperationException("No pending transactions found for the sender address.");
            var totalPendingAmount = PendingTransactions
                .Sum(t => t.Amount);
            if (totalPendingAmount + transaction.Amount > GetBalance(transaction.FromAddress))
                throw new InvalidOperationException("Insufficient balance for the transaction.");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public virtual void AddTransaction(ITransact transaction)
    {
        var verify = VerifyTransaction(transaction);
        if (!verify)
        {
            throw new InvalidOperationException("Transaction verification failed.");
        }
        PendingTransactions.Add(transaction);
    }

    private decimal CalculateAddressBalance(string address) =>
        Chain.SelectMany(block => block.TransactionsReadOnly)
            .Sum(transaction =>
                (transaction.ToAddress == address ? transaction.Amount : 0) -
                (transaction.FromAddress == address ? transaction.Amount : 0));

    public decimal GetBalance(string address)
    {
        if (string.IsNullOrEmpty(address)) throw new ArgumentException("Address cannot be null or empty.", nameof(address));
        return CalculateAddressBalance(address);
    }

    public void ValidateChain()
    {
        for (var i = 1; i < Chain.Count; i++)
        {
            var currentBlock = Chain[i];
            var previousBlock = Chain[i - 1];

            if (currentBlock.PreviousHash != previousBlock.Hash)
            {
                throw new InvalidOperationException($"Invalid chain: Block {i} has an incorrect previous hash.");
            }

            if (!currentBlock.ValidBlock(Difficulty))
            {
                throw new InvalidOperationException($"Invalid block: Block {i} is not valid.");
            }
        }
    }
}

public class TestChain : Blockchain, IBlockchain
{
    public string Name { get; }
    public int Difficulty { get; }
    public decimal MiningReward { get; }
    public TestChain()
    {
        Name = "Test Chain";
        Difficulty = 2;
        MiningReward = 50;
        Chain.Add(CreateGenesisBlock());
    }
    private Block CreateGenesisBlock()
    {
        // Create the first block in the blockchain with a default previous hash of "0"
        var genesisTransaction = new Transaction("System", "0", 1);
        PendingTransactions.Add(genesisTransaction);
        return new Block("0", PendingTransactions);
    }
    private List<ITransact> PendingTransactions { get;  } = new();
    public IReadOnlyList<ITransact> GetPendingTransactions() => PendingTransactions.AsReadOnly();
    private List<IBlock> Chain { get;  } = new();
    public IReadOnlyList<IBlock> GetChain() => Chain.AsReadOnly();
    public IBlock GetLatestBlock() => GetChain().Last();
    public override void AddBlock(IBlock block) { }
    public override void AddTransaction(ITransact transaction) { }
    public decimal GetBalance(string walletAddress) => 0;
    public void ValidateChain() { }
}