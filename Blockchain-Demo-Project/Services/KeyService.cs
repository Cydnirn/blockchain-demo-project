using System.Security.Cryptography;

namespace Blockchain_Demo_Project.Services;

internal abstract class KeyGenerator
{
    public abstract (byte[] publicKey, byte[] privateKey) GenerateKeyPair();
    public abstract byte[] ExportKeyPair(string privateKey);
}


/// Generates cryptographic key pairs for wallets using the secp256k1 curve
internal class EcdsaKeyService: KeyGenerator
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