using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project.Blockchain;

public class Block(string previousHash, IReadOnlyList<ITransact> transactions) : IBlock
{
    public string PreviousHash { get; } = previousHash;
    public string Hash { get; private set; } = "0"; // Default hash value, will be updated after mining
    private string Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    private List<ITransact> Transactions { get; } = [..transactions];
    public IReadOnlyList<ITransact> TransactionsReadOnly { get; } = [..transactions];
    public int Nonce { get; private set; }
    public void IncrementNonce() => Nonce++;
    public void GenerateHash()
    {
        // Concatenate the previous hash, timestamp, nonce, and transaction signatures to create a unique hash for the block
        Hash = Convert.ToHexString(SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(
                PreviousHash + Timestamp + Nonce + string.Join(
                    "", Transactions.Select(t => t.Signature)
                    )
                )
            )
        );
    }

    public bool ValidBlock(int difficulty = 2)
    {
        // Check if the block's hash starts with the required number of leading zeros based on the difficulty
        var validHash = Hash.StartsWith(new string('0', difficulty));

        // All transactions must be valid
        var validTx = TransactionsReadOnly.All(tx => tx.VerifySignature());

        return validHash && validTx;
    }
}