namespace Blockchain_Demo_Project.Interfaces;

public interface ITransact
{
    string FromAddress { get;  }
    string ToAddress { get;  }
    decimal Amount { get;  }
    string TimeStamp { get;  } // In UNIX timestamp format
    string Signature { get;  }
    bool VerifySignature(); // Method to verify the signature of the transaction
    void SignTransaction(string privateKey); // Method to sign the transaction with the private key
}