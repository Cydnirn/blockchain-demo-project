using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project;


public class Block(string previousHash, List<ITransact> transactions) : IBlock
{
    public string PreviousHash { get; } = previousHash;
    public string Hash { get; private set; } = "0"; // Default hash value, will be updated after mining
    private string Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    private List<ITransact> Transactions { get; } = [..transactions];
    public IReadOnlyList<ITransact> TransactionsReadOnly { get; } = [..transactions.AsReadOnly()];
    public int Nonce { get; set; }


    public void GenerateHash()
    {
        // Concatenate the previous hash, timestamp, nonce, and transaction signatures to create a unique hash for the block
        Hash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(PreviousHash + Timestamp + Nonce + string.Join("", Transactions.Select(t => t.Signature)))));
    }

    public virtual bool ValidBlock()
    {
        foreach (var transaction in TransactionsReadOnly)
        {
            if(!transaction.VerifySignature()) return false;
        }
        return true;
    }
}