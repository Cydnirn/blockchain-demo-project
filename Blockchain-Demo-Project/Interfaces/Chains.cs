namespace Blockchain_Demo_Project.Interfaces;

public struct Chains
{
    public IBlockchain Mainnet { get; init; }
    public IBlockchain Testnet { get; init; }
}