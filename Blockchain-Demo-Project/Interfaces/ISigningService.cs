namespace Blockchain_Demo_Project.Interfaces;

public interface ISigningService
{
    string Sign(string hash);
    bool Verify(string signature, string hash, string publicKey);
}