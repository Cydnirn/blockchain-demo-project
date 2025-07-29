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
    protected abstract List<ITransact> PendingTransactions { get; }
    public virtual IReadOnlyList<ITransact> GetPendingTransactions() => PendingTransactions.AsReadOnly();
    protected abstract List<IBlock> Chain { get; }
    public virtual decimal GetMiningReward() => MiningReward;
    public abstract IReadOnlyList<IBlock> GetChain();
    protected virtual IBlock CreateGenesisBlock()
    {
        var genesisTransaction = new Transaction("System", "0", 1);
        PendingTransactions.Add(genesisTransaction);
        return new Block("0", PendingTransactions);
    }
    public virtual IBlock GetLatestBlock() => Chain.Last() ?? throw new InvalidOperationException("Blockchain is empty.");
    protected abstract void ClearPendingTransactions();
    public virtual void AddBlock(IBlock block)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(block);
            if (!block.ValidBlock(Difficulty)) throw new InvalidOperationException("Invalid block.");
            Chain.Add(block);
            ClearPendingTransactions();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public virtual void AddTransaction(ITransact transact)
    {
        ArgumentNullException.ThrowIfNull(transact);
        if (!VerifyTransaction(transact))
        {
            throw new InvalidOperationException("Transaction is invalid.");
        }
        PendingTransactions.Add(transact);
    }
    public virtual void AddTransaction(ITransact[] transacts)
    {
        ArgumentNullException.ThrowIfNull(transacts);
        if (transacts.Any(tx => !VerifyTransaction(tx)))
        {
            throw new InvalidOperationException("Transaction is invalid.");
        }
        PendingTransactions.AddRange(transacts);
    }
    protected abstract bool VerifyTransaction(ITransact transaction);
    public virtual decimal GetBalance(string walletAddress) => CalculateAddressBalance(walletAddress);
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