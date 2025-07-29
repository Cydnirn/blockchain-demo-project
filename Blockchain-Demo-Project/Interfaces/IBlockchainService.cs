namespace Blockchain_Demo_Project.Interfaces;

public interface IBlockchainService
{
    void InitializeBlockchain(string address);
    void AddTransaction(ITransact transaction, string privateKey);
    decimal GetBalance(string walletAddress);
    IReadOnlyList<IBlock> GetChain();
    string GetChainName();
    bool IsChainValid();
}