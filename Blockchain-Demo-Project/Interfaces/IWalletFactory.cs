namespace Blockchain_Demo_Project.Interfaces;

public interface IWalletFactory
{
    public IWallet CreateWallet();
    public IWallet ExportWallet(string privateKey);
}