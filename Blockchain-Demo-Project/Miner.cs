using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project;

public class Miner(string walletAddress) : IMiner
{
    private string WalletAddress { get; } = walletAddress;

    public static Miner Create(string walletAddress)
    {
        return new Miner(walletAddress);
    }

    // Method to mine a new block and add it to the blockchain
    public void MineBlock(IBlockchain blockchain)
    {
        if (blockchain.GetPendingTransactions().Count == 0)
        {
            Console.WriteLine("No pending transactions to mine.");
            return;
        }

        var rewardTransaction = new MinerTransaction( WalletAddress, blockchain.MiningReward);

        //Add the mining reward transaction to the pending transactions
        blockchain.AddTransaction(rewardTransaction);

        var previousBlock = blockchain.GetLatestBlock();
        var newBlock = new Block(previousBlock.Hash, blockchain.GetPendingTransactions());

        // Simulate mining by finding a valid nonce
        while (!newBlock.Hash.StartsWith(new string('0', blockchain.Difficulty)))
        {
            newBlock.IncrementNonce();
            newBlock.GenerateHash();
        }

        // Add the mined block to the blockchain
        blockchain.AddBlock(newBlock);
    }
}