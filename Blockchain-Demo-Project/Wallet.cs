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
    private readonly ECDsa _instance = ECDsa.Create(ECCurve.CreateFromFriendlyName(CurveName));

    // Generates a new key pair using the secp256k1 curve.
    // Returns a tuple containing the public key and private key in byte arrays.
    public override (byte[] publicKey, byte[] privateKey) GenerateKeyPair()
    {
        var privateKey = _instance.ExportECPrivateKey();
        var publicKey = _instance.ExportSubjectPublicKeyInfo();

        return (publicKey, privateKey);
    }

    /// Exports the public key from a given private key in hexadecimal format.
    /// The private key is expected to be in hexadecimal string format.
    /// Returns the public key in SubjectPublicKeyInfo format.
    public override byte[] ExportKeyPair(string privateKey)
    {
        _instance.ImportECPrivateKey(Convert.FromHexString(privateKey), out _);
        return _instance.ExportSubjectPublicKeyInfo();
    }

    // Signs a message using the private key and returns the signature.
    protected byte[] SignData(byte[] message, byte[] privateKey, HashAlgorithmName hashAlgorithm)
    {
        try
        {
            _instance.ImportECPrivateKey(privateKey, out _);
            return _instance.SignData(message, hashAlgorithm);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error signing data: " + e.Message);
            throw;
        }
    }

    // Verifies a digital signature using the public key and the original message.
    // Returns true if the signature is valid, false otherwise.
    protected bool VerifySignature(byte[] signature, byte[] message, byte[] publicKey, HashAlgorithmName hashAlgorithm )
    {
        try
        {
            _instance.ImportSubjectPublicKeyInfo(publicKey, out _);
            return _instance.VerifyData(message, signature, hashAlgorithm);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error verifying signature: " + e.Message);
            return false;
        }
    }
}

/// Represents a wallet in the blockchain system.
/// Contains a public key and a private key for cryptographic operations.
/// The public key is used to identify the wallet, while the private key is used for signing
/// transactions and proving ownership of the wallet's funds.
public class Wallet(string publicKey, string privateKey)
{
    public string PublicKey { get; } = publicKey;
    public string PrivateKey { get; } = privateKey;

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