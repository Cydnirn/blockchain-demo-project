using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project;

public class Miner(string walletAddress)
{
    public string WalletAddress { get; } = walletAddress;

    public static Miner Create(string walletAddress)
    {
        return new Miner(walletAddress);
    }

    // Method to mine a new block and add it to the blockchain
    public void MineBlock(Blockchain blockchain)
    {
        if (blockchain.GetPendingTransactions().Count == 0)
        {
            throw new InvalidOperationException("No transactions to mine.");
        }

        var rewardTransaction = new MinerTransaction( WalletAddress, blockchain.MiningReward);

        //Add the mining reward transaction to the pending transactions
        blockchain.AddTransaction(rewardTransaction);

        var previousBlock = blockchain.GetLatestBlock();
        var newBlock = new Block(previousBlock.Hash, blockchain.GetPendingTransactions());

        // Simulate mining by finding a valid nonce
        while (!newBlock.Hash.StartsWith(new string('0', blockchain.Difficulty)))
        {
            newBlock.Nonce++;
            newBlock.Hash = newBlock.GenerateHash();
        }

        // Add the mined block to the blockchain
        blockchain.AddBlock(newBlock);
    }
}