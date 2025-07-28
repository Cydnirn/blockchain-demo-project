namespace Blockchain_Demo_Project.Interfaces;

public interface IBlock
{
    public string PreviousHash { get; }
    public string Hash { get; }
    public IReadOnlyList<ITransact> TransactionsReadOnly { get; }
    public int Nonce { get; set; }
    public void GenerateHash();
    public bool ValidBlock();
}