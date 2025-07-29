namespace Blockchain_Demo_Project.Interfaces;

public struct Chains
{
    public IBlockchain Mainnet { get; set; }
    public IBlockchain Testnet { get; set; }
}