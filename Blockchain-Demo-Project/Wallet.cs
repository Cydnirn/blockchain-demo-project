using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;
using Blockchain_Demo_Project.Services;

namespace Blockchain_Demo_Project;

/// Represents a wallet in the blockchain system.
/// Contains a public key and a private key for cryptographic operations.
/// The public key is used to identify the wallet, while the private key is used for signing
/// transactions and proving ownership of the wallet's funds.
public class Wallet(string publicKey, string privateKey)
{
    public string PublicKey { get; } = publicKey;
    private string PrivateKey { get; } = privateKey;

    internal static Wallet Create()
    {
        var keyGenerator = new EcdsaKeyService();
        var factory = new WalletFactory(keyGenerator);
        return factory.CreateWallet();
    }

    internal static Wallet Export(string privateKey)
    {
        var keyGenerator = new EcdsaKeyService();
        var factory = new WalletFactory(keyGenerator);
        return factory.ExportWallet(privateKey);
    }

    public string GetPrivateKey()
    {
        if (string.IsNullOrEmpty(PrivateKey))
        {
            throw new InvalidOperationException("Private key is not set or is empty.");
        }
        return PrivateKey;
    }
}

/// Factory class responsible for creating wallet instances
internal class WalletFactory (KeyGenerator keyGenerator) : IWalletFactory
{
    public Wallet CreateWallet()
    {
        try
        {
            var (publicKey, privateKey) = keyGenerator.GenerateKeyPair();
            return new Wallet(Convert.ToHexString(publicKey), Convert.ToHexString(privateKey));
        }
        catch (Exception e)
        {
            Console.WriteLine("Error generating wallet: " + e.Message);
            throw;
        }
    }

    public Wallet ExportWallet(string privateKey)
    {
        try
        {
            var publicKey = keyGenerator.ExportKeyPair(privateKey);
            return new Wallet(Convert.ToHexString(publicKey), privateKey);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error exporting wallet: " + e.Message);
            throw;
        }
    }
}