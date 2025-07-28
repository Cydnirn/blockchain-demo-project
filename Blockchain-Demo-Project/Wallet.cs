using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project;

internal abstract class KeyGenerator
{
    public abstract (byte[] publicKey, byte[] privateKey) GenerateKeyPair();
    public abstract byte[] ExportKeyPair(string privateKey);
}

/// Generates cryptographic key pairs for wallets using the secp256k1 curve
internal class EcdsaKeyGenerator : KeyGenerator
{
    private const string CurveName = "secp256k1";

    public override (byte[] publicKey, byte[] privateKey) GenerateKeyPair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.CreateFromFriendlyName(CurveName));
        var privateKey = ecdsa.ExportECPrivateKey();
        var publicKey = ecdsa.ExportSubjectPublicKeyInfo();

        return (publicKey, privateKey);
    }

    /// Exports the public key from a given private key in hexadecimal format.
    /// The private key is expected to be in hexadecimal string format.
    /// Returns the public key in SubjectPublicKeyInfo format.
    public override byte[] ExportKeyPair(string privateKey)
    {
        using var ecdsa = ECDsa.Create(ECCurve.CreateFromFriendlyName(CurveName));
        ecdsa.ImportECPrivateKey(Convert.FromHexString(privateKey), out _);
        return ecdsa.ExportSubjectPublicKeyInfo();
    }
}

/// Represents a wallet in the blockchain system.
/// Contains a public key and a private key for cryptographic operations.
/// The public key is used to identify the wallet, while the private key is used for signing
/// transactions and proving ownership of the wallet's funds.
public class Wallet
{
    public string PublicKey { get; }
    public string PrivateKey { get; }

    public Wallet(string publicKey, string privateKey)
    {
        PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
        PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
    }

    internal static Wallet Create()
    {
        var keyGenerator = new EcdsaKeyGenerator();
        var factory = new WalletFactory(keyGenerator);
        return factory.CreateWallet();
    }

    internal static Wallet Export(string privateKey)
    {
        var keyGenerator = new EcdsaKeyGenerator();
        var factory = new WalletFactory(keyGenerator);
        return factory.ExportWallet(privateKey);
    }
}

/// Factory class responsible for creating wallet instances
internal class WalletFactory (KeyGenerator keyGenerator)
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