using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project;

public class Transaction(string fromAddress, string toAddress, decimal amount) : ITransact
{
    private class Signing : EcdsaKeyGenerator
    {
        private readonly byte[]? _privateKey;

        private Signing(string? privateKey = null)
        {
            _privateKey = string.IsNullOrEmpty(privateKey) ? null : Convert.FromHexString(privateKey);
        }

        public static Signing Create(string privateKey)
        {
            return new Signing(privateKey);
        }

        public static Signing Create()
        {
            return new Signing();
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
    public string FromAddress { get; set; } = fromAddress;
    public string ToAddress { get; set; } = toAddress;
    public decimal Amount { get; set; } = amount;
    public string TimeStamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    public string Signature { get; set; } = string.Empty;
    public string CalculateHash()
    {
        return Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes($"{FromAddress}:{ToAddress}:{Amount}:{TimeStamp}")));
    }

    public void SignTransaction(string privateKey)
    {
        try
        {
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new ArgumentException("Private key cannot be null or empty.", nameof(privateKey));
            }
            var signing = Signing.Create(privateKey);
            var hash = CalculateHash();

            Signature = signing.Sign(hash);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error signing transaction: {ex.Message}");
        }
    }

    public virtual bool VerifySignature()
    {
        try
        {
            if (string.IsNullOrEmpty(Signature))
            {
                throw new InvalidOperationException("Signature is not set or is empty.");
            }
            var signing = Signing.Create();
            return signing.Verify(Signature, CalculateHash(), FromAddress);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying signature: {ex.Message}");
            return false;
        }

    }
}

public class MinerTransaction : Transaction
{
    public MinerTransaction(string toAddress, decimal amount) : base("Miner", toAddress, amount)
    {
        // Miner transactions are typically created with a fixed "from" address of "Miner"
        // and a specified "to" address and amount.
        Signature = string.Empty;
    }

    public override bool VerifySignature()
    {
        // Miner transactions do not require signature verification
        return true;
    }
}