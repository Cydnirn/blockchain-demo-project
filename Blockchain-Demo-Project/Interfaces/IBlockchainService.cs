namespace Blockchain_Demo_Project.Interfaces;

public interface IBlockchainService
{
    void AddTransaction(ITransact transaction, string privateKey);
    decimal GetBalance(string walletAddress);
}