using Blockchain_Demo_Project.Interfaces;

namespace Blockchain_Demo_Project.Services;

public class ConsoleService(IBlockchainService svc, Chains network) : IConsoleService
{
    public IBlockchainService Service { get; private set; } = svc;
    private IWallet? WalletSelf { get; set; }
    private static ConsoleService? _instance;
    public static IConsoleService Create(IBlockchainService service, Chains network)
    {
        _instance ??= new ConsoleService(service, network);
        return _instance;
    }

    public void InitializeChains()
    {
        if (WalletSelf != null)
        {
            Service.InitializeBlockchain(WalletSelf.PublicKey);
        }
    }

    public IWallet? GetWallet()
    {
        return WalletSelf;
    }
    public void SelectChain()
    {
        Console.Clear();
        Console.WriteLine("Select a blockchain to work with:");
        Console.WriteLine("1. Default Blockchain");
        Console.WriteLine("2. Test Chain");
        Console.WriteLine("Press 'q' to quit or any other key to continue...");
        var input = Console.ReadKey(true).KeyChar;
        switch (input)
        {
            case '1':
                Service = BlockchainService.Create(network.Mainnet);
                Console.WriteLine("Switched to Default Blockchain.");
                break;
            case '2':
                Service = BlockchainService.Create(network.Testnet);
                Console.WriteLine("Switched to Test Chain.");
                break;
            case 'q':
            case 'Q':
                return;
            default:
                Console.WriteLine("Invalid selection. Please try again.");
                break;
        }
        Console.ReadKey();
    }

    public void CreateWallet()
    {
        if (WalletSelf != null)
        {
            Console.WriteLine("You are already logged in with a wallet. Please log out first.");
            Console.ReadKey();
            return;
        }
        WalletSelf = Wallet.Create();
        Console.WriteLine("New wallet created with Public Key: " + WalletSelf.PublicKey);
        Console.WriteLine("Private Key: " + WalletSelf.GetPrivateKey());
        Console.ReadKey();
    }

    public void ExportWallet()
    {
        if (WalletSelf != null)
        {
            Console.WriteLine("Current wallet is still active. Please log out first.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Enter the private key to export the wallet:");
        var privateKey = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(privateKey))
        {
            Console.WriteLine("Private key cannot be empty.");
            Console.ReadKey();
            return;
        }
        try
        {
            WalletSelf = Wallet.Export(privateKey);
            Console.WriteLine("Wallet exported successfully!");
            Console.WriteLine("Public Key: " + WalletSelf.PublicKey);
            Console.WriteLine("Private Key: " + WalletSelf.GetPrivateKey());
        } catch (Exception ex)
        {
            Console.WriteLine("Error exporting wallet: " + ex.Message);
            return;
        }
        Console.ReadKey();
    }

    public void LogOutWallet()
    {
        if(WalletSelf == null)
        {
            Console.WriteLine("No wallet created. Please create a wallet first.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Are you sure you want to log out from the current wallet? (y/n)");
        var confirmation = Console.ReadKey(true).KeyChar;
        if (confirmation == 'y' || confirmation == 'Y')
        {
            WalletSelf = null;
            Console.WriteLine("Current wallet log out successfully.");
        }
        else
        {
            Console.WriteLine("Log out cancelled.");
        }
        Console.ReadKey();
    }
    public void SendTx()
    {
        if (WalletSelf == null)
        {
            Console.WriteLine("No wallet created. Please create a wallet first.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Enter the recipient's address:");
        var toAddress = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(toAddress))
        {
            Console.WriteLine("Recipient address cannot be empty.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Enter the amount of WiwokCoin to send:");
        if (!decimal.TryParse(Console.ReadLine()?.Trim(), out var amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount. Please enter a valid decimal number greater than zero.");
            Console.ReadKey();
            return;
        }
        var transaction = new Transaction(WalletSelf.PublicKey, toAddress, amount);
        try
        {
            Service.AddTransaction(transaction, WalletSelf.GetPrivateKey());
            Console.WriteLine("Transaction added successfully!");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding transaction: " + ex.Message);
            Console.ReadKey();
        }
    }
    public void CheckBalance()
    {
        if (WalletSelf == null)
        {
            Console.WriteLine("No wallet created. Please create a wallet first.");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Balance for wallet: " + Service.GetBalance(WalletSelf.PublicKey));
        Console.ReadKey();
    }
    public void GetChain()
    {
        var chain = Service.GetChain();
        if(chain.Count == 0)
        {
            Console.WriteLine("Blockchain is empty.");
        }
        foreach (var block in chain)
        {
            Console.WriteLine("-- Block --");
            Console.WriteLine($"Block Hash: {block.Hash}");
            Console.WriteLine($"Previous Hash: {block.PreviousHash}");
            Console.WriteLine($"Nonce: {block.Nonce}");
            Console.WriteLine("Transactions:");
            foreach (var tx in block.TransactionsReadOnly)
            {
                Console.WriteLine($"  From: {tx.FromAddress}, To: {tx.ToAddress}, Amount: {tx.Amount}, Signature: {tx.Signature}");
            }
            Console.WriteLine("-------------------");
        }
        Console.ReadKey();
    }
}