using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project.Blockchain;

public abstract class BlockchainBase : IBlockchain
{
    public abstract string Name { get; protected set; }
    public abstract int Difficulty { get; protected set; }
    protected abstract decimal MiningReward { get; set; }
    public virtual decimal GetFee()
    {
        return 0;
    }
    protected virtual List<ITransact> PendingTransactions { get; } = new();
    public virtual IReadOnlyList<ITransact> GetPendingTransactions() => PendingTransactions.AsReadOnly();
    private List<IBlock> Chain { get; } = new();
    public virtual decimal GetMiningReward() => MiningReward;
    public abstract IReadOnlyList<IBlock> GetChain();
    protected abstract Block CreateGenesisBlock();
    public virtual IBlock GetLatestBlock() => Chain.Last() ?? throw new InvalidOperationException("Blockchain is empty.");
    protected abstract void ClearPendingTransactions();
    public virtual void AddBlock(IBlock block)
    {
        Chain.Add(block);
    }
    public virtual void AddTransaction(ITransact transact)
    {
        PendingTransactions.Add(transact);
    }

    public virtual void AddTransaction(ITransact[] transacts)
    {
        PendingTransactions.AddRange(transacts);
    }
    protected abstract bool VerifyTransaction(ITransact transaction);
    public abstract decimal GetBalance(string walletAddress);
    public virtual void ValidateChain()
    {
        for(var i = 1; i < Chain.Count; i++)
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
    public abstract bool IsValidChain();
    protected virtual decimal CalculateAddressBalance(string address) =>
        Chain.SelectMany(block => block.TransactionsReadOnly)
            .Sum(transaction =>
                (transaction.ToAddress == address ? transaction.Amount : 0) -
                (transaction.FromAddress == address ? transaction.Amount : 0));

}