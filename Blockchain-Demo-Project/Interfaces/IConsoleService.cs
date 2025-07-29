namespace Blockchain_Demo_Project.Interfaces;

public interface IConsoleService
{
    public IWallet? GetWallet();
    public void InitializeChains();
    public void SelectChain();
    public void CreateWallet();
    public void ExportWallet();
    public void LogOutWallet();
    public void SendTx();
    public void CheckBalance();
    public void GetChain();
    public string GetChainName();
}