using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;
namespace Blockchain_Demo_Project.Services;

internal class SigningService : EcdsaKeyService, ISigningService
{
    private readonly byte[]? _privateKey;
    private SigningService(string? privateKey = null)
    {
        _privateKey = string.IsNullOrEmpty(privateKey) ? null : Convert.FromHexString(privateKey);
    }

    public static SigningService Create(string privateKey)
    {
        return new SigningService(privateKey);
    }

    public static SigningService Create()
    {
        return new SigningService();
    }

    public bool Verify(string signature,string hash, string publicKey)
    {
        return VerifySignature(
            Convert.FromHexString(signature),
            Convert.FromBase64String(hash),
            Convert.FromHexString(publicKey),
            HashAlgorithmName.SHA256);
    }

    public string Sign(string hash)
    {
        if (_privateKey == null || _privateKey.Length == 0)
        {
            throw new InvalidOperationException("Private key is not set or is empty.");
        }

        var message = Convert.FromBase64String(hash);
        var signature = SignData(message, _privateKey, HashAlgorithmName.SHA256);
        return Convert.ToHexString(signature);
    }
}