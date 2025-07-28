namespace Blockchain_Demo_Project.Interfaces;

public interface IBlockchain
{
    public string Name { get; }
    public int Difficulty { get; }
    public decimal MiningReward { get; }
    public IReadOnlyList<ITransact> GetPendingTransactions();
    public IReadOnlyList<IBlock> GetChain();
    public IBlock GetLatestBlock();
    public void AddBlock(IBlock block);
    public void AddTransaction(ITransact transaction);
    public decimal GetBalance(string walletAddress);
    public void ValidateChain();
}