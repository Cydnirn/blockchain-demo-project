using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;
using Blockchain_Demo_Project.Services;

namespace Blockchain_Demo_Project;

public class Transaction(string fromAddress, string toAddress, decimal amount) : ITransact
{
    public string FromAddress { get; private set; } = fromAddress;
    public string ToAddress { get; private set; } = toAddress;
    public decimal Amount { get; private set; } = amount;
    public string TimeStamp { get; private set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    public string Signature { get; private set; } = string.Empty;
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
            var signing = SigningService.Create(privateKey);
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
            if(FromAddress == "System")
            {
                // Miner transactions do not require signature verification
                return true;
            }
            if (string.IsNullOrEmpty(Signature))
            {
                throw new InvalidOperationException("Signature is not set or is empty.");
            }
            var signing = SigningService.Create();
            return signing.Verify(Signature, CalculateHash(), FromAddress);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying signature: {ex.Message}");
            return false;
        }

    }
}

public class RewardTransaction(string toAddress, decimal amount) : Transaction("System", toAddress, amount)
{
    public override bool VerifySignature()
    {
        // Miner transactions do not require signature verification
        return true;
    }
}