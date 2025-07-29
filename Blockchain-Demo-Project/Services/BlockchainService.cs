using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project.Services;

public class BlockchainService: IBlockchainService
{
    private IBlockchain Blockchain { get;  }

    private BlockchainService(IBlockchain blockchain)
    {
        Blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
    }

    public static BlockchainService Create(IBlockchain blockchain)
    {
        return new BlockchainService(blockchain);
    }

    private void ExecuteMiner(string address)
    {
        var miner = Miner.Create(address);
        var minerThread = new Thread(() => miner.MineBlock(Blockchain));
        minerThread.Start();
    }

    public void InitializeBlockchain(string address)
    {
        if (GetChain().Count == 1)
        {
            ExecuteMiner(address);
        }
    }

    public void AddTransaction(ITransact transaction, string privateKey)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (string.IsNullOrEmpty(privateKey)) throw new ArgumentException("Private key cannot be null or empty.", nameof(privateKey));
        if (Blockchain.GetFee() > 0)
        {
            ITransact txWithFee = new Transaction(transaction.FromAddress, "System", Blockchain.GetFee());
            transaction.SignTransaction(privateKey);
            txWithFee.SignTransaction(privateKey);
            Blockchain.AddTransaction([transaction, txWithFee]);
        }
        else
        {
            transaction.SignTransaction(privateKey);
            Blockchain.AddTransaction(transaction);
        }
        ExecuteMiner(transaction.FromAddress);
    }

    public decimal GetBalance(string walletAddress)
    {
        return Blockchain.GetBalance(walletAddress);
    }

    public IReadOnlyList<IBlock> GetChain() => Blockchain.GetChain();

    public string GetChainName() => Blockchain.Name;
    public bool IsChainValid()
    {
        return Blockchain.IsValidChain();
    }
}