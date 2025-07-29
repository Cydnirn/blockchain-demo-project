namespace Blockchain_Demo_Project.Interfaces;

public interface IWallet
{
    public string PublicKey { get; }
    public string GetPrivateKey();

    public static abstract IWallet Create();
    public static abstract IWallet Export(string privateKey);
}