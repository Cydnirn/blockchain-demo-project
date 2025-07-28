namespace Blockchain_Demo_Project.Interfaces;

public interface ITransact
{
    string FromAddress { get; set; }
    string ToAddress { get; set; }
    decimal Amount { get; set; }
    string TimeStamp { get; set; } // In UNIX timestamp format
    string Signature { get; set; }
    string CalculateHash(); // Method to calculate the hash of the transaction
    bool VerifySignature(); // Method to verify the signature of the transaction
    void SignTransaction(string privateKey); // Method to sign the transaction with the private key
}