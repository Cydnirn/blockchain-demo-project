using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project;

public class Blockchain
{
    private List<IBlock> Chain {get; } = new();
    public int Difficulty { get; } = 2;
    public decimal MiningReward { get; private set; } = 100;
    private List<ITransact> PendingTransactions { get; set; } = new();

    public Blockchain()
    {
        Chain.Add(CreateGenesisBlock());
    }

    private Block CreateGenesisBlock()
    {
        // Create the first block in the blockchain with a default previous hash of "0"
        var genesisTransaction = new Transaction("System", "0", 1);
        PendingTransactions.Add(genesisTransaction);
        return new Block("0", PendingTransactions);
    }

    public IBlock GetLatestBlock()
    {
        return Chain.Last();
    }

    public IReadOnlyList<IBlock> GetChain()
    {
        return Chain;
    }

    public List<ITransact> GetPendingTransactions()
    {
        return PendingTransactions;
    }

    private void ClearPendingTransactions()
    {
        PendingTransactions.Clear();
    }

    public void AddBlock(Block block)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        if (!block.ValidBlock()) throw new InvalidOperationException("Invalid block.");

        Chain.Add(block);
        ClearPendingTransactions();
    }

    public void AddTransaction(ITransact transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (string.IsNullOrEmpty(transaction.FromAddress) || string.IsNullOrEmpty(transaction.ToAddress))
            throw new ArgumentException("Transaction must have valid From and To addresses.");

        if (transaction.Amount <= 0)
            throw new ArgumentException("Transaction amount must be greater than zero.", nameof(transaction.Amount));
        if (transaction.FromAddress == "System")
        {
            PendingTransactions.Add(transaction);
            return; // System transactions (like mining rewards) do not require balance checks
        }
        if (!transaction.VerifySignature())
            throw new InvalidOperationException("Transaction signature is invalid.");
        if (GetBalance(transaction.FromAddress) < transaction.Amount)
            throw new InvalidOperationException("Insufficient balance for the transaction.");

        var pendingTx = PendingTransactions.FindAll(t => t.FromAddress == transaction.FromAddress);
        if (pendingTx.Count > 0)
        {
            var totalPendingAmount = PendingTransactions
                .Sum(t => t.Amount);
            if (totalPendingAmount + transaction.Amount > GetBalance(transaction.FromAddress))
                throw new InvalidOperationException("Insufficient balance for the transaction.");
        }
        PendingTransactions.Add(transaction);
    }

    public decimal GetBalance(string address)
    {
        if (string.IsNullOrEmpty(address)) throw new ArgumentException("Address cannot be null or empty.", nameof(address));

        var balance = 0m;
        foreach (var block in Chain)
        {
            foreach (var transaction in block.TransactionsReadOnly)
            {
                if (transaction.ToAddress == address)
                {
                    balance += transaction.Amount;
                }
                if (transaction.FromAddress == address)
                {
                    balance -= transaction.Amount;
                }
            }
        }
        return balance;
    }
}

public class BlockchainService(Blockchain blockchain) : IBlockchainService
{
    private Blockchain Blockchain { get; } = blockchain;
    private static BlockchainService? _instance;

    public static BlockchainService Create(Blockchain blockchain)
    {
        if (_instance != null) return _instance;

        lock (typeof(BlockchainService))
        {
            _instance ??= new BlockchainService(blockchain);
        }

        return _instance;
    }

    private void ExecuteMiner(string address)
    {
        var miner = Miner.Create(address);
        var minerThread = new Thread(() => miner.MineBlock(Blockchain));
        minerThread.Start();
    }

    public void AddTransaction(ITransact transaction, string privateKey)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (string.IsNullOrEmpty(privateKey)) throw new ArgumentException("Private key cannot be null or empty.", nameof(privateKey));
        transaction.SignTransaction(privateKey);
        Blockchain.AddTransaction(transaction);
        ExecuteMiner(transaction.FromAddress);
    }

    public decimal GetBalance(string walletAddress)
    {
        return Blockchain.GetBalance(walletAddress);
    }
}