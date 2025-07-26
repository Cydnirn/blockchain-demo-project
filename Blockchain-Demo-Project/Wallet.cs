namespace Blockchain_Demo_Project;
using System.Security.Cryptography;

public interface IWallet
{
    string PublicKey { get; }
    string PrivateKey { get; }
}

public interface IKeyGenerator
{
    (byte[] publicKey, byte[] privateKey) GenerateKeyPair();
}

/// Generates cryptographic key pairs for wallets using the secp256k1 curve
public class KeyGenerator : IKeyGenerator
{
    private const string CurveName = "secp256k1";

    public (byte[] publicKey, byte[] privateKey) GenerateKeyPair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.CreateFromFriendlyName(CurveName));
        var privateKey = ecdsa.ExportECPrivateKey();
        var publicKey = ecdsa.ExportSubjectPublicKeyInfo();

        return (publicKey, privateKey);
    }
}

/// Represents a wallet in the blockchain system.
/// Contains a public key and a private key for cryptographic operations.
/// The public key is used to identify the wallet, while the private key is used for signing
/// transactions and proving ownership of the wallet's funds.
public class Wallet : IWallet
{
    public string PublicKey { get; }
    public string PrivateKey { get; }

    public Wallet(string publicKey, string privateKey)
    {
        PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
        PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
    }

    public static IWallet Create()
    {
        var keyGenerator = new KeyGenerator();
        var factory = new WalletFactory(keyGenerator);
        return factory.CreateWallet();
    }
}

/// Factory class responsible for creating wallet instances
public class WalletFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public WalletFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
    }

    public IWallet CreateWallet()
    {
        try
        {
            var (publicKey, privateKey) = _keyGenerator.GenerateKeyPair();
            return new Wallet(Convert.ToHexString(publicKey), Convert.ToHexString(privateKey));
        }
        catch (Exception e)
        {
            Console.WriteLine("Error generating wallet: " + e.Message);
            throw;
        }
    }
}