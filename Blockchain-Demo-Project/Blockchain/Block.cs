using System.Security.Cryptography;
using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project.Blockchain;

public abstract class BlockBase : IBlock
{
    public abstract string PreviousHash { get; }
    public abstract string Hash { get; protected set; }
    public abstract IReadOnlyList<ITransact> TransactionsReadOnly { get; }
    public abstract int Nonce { get; protected set; }
    public abstract void GenerateHash();
    public abstract bool ValidBlock(int difficulty = 2);
}

public class Block(string previousHash, IReadOnlyList<ITransact> transactions) : BlockBase
{
    public override string PreviousHash { get; } = previousHash;
    public override string Hash { get; protected set; } = "0"; // Default hash value, will be updated after mining
    private string Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    private List<ITransact> Transactions { get; } = [..transactions];
    public override IReadOnlyList<ITransact> TransactionsReadOnly { get; } = [..transactions];
    public override int Nonce { get; protected set; }
    public void IncrementNonce() => Nonce++;
    public override void GenerateHash()
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

    public override bool ValidBlock(int difficulty = 2)
    {
        // Check if the block's hash starts with the required number of leading zeros based on the difficulty
        var validHash = Hash.StartsWith(new string('0', difficulty));

        // All transactions must be valid
        var validTx = TransactionsReadOnly.All(tx => tx.VerifySignature());

        return validHash && validTx;
    }
}