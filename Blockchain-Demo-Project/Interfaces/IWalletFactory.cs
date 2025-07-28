namespace Blockchain_Demo_Project.Interfaces;

public interface IWalletFactory
{
    public Wallet CreateWallet();
    public Wallet ExportWallet(string privateKey);
}