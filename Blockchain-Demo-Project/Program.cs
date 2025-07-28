// See https://aka.ms/new-console-template for more information
using Blockchain_Demo_Project;

Wallet? wallet = null;
var blockchain = new Blockchain();
var firstIteration = true;

var service = BlockchainService.Create(blockchain);

while (true)
{
    //Initiate the first WiwokCoin wallet
    if (wallet != null && firstIteration)
    {
        var miner = Miner.Create(wallet.PublicKey);
        var minerThread = new Thread(() => miner.MineBlock(blockchain));
        minerThread.Start();
        firstIteration = false;
    }

    Console.Clear();
    Console.WriteLine("Blockchain Demo Project");
    Console.WriteLine("Current Wallet: " + (wallet != null ? wallet.PublicKey : "No wallet created"));
    Console.WriteLine("1. Create a new wallet");
    Console.WriteLine("2. Export an existing wallet");
    Console.WriteLine("3. Log out from the current wallet");
    Console.WriteLine("4. Send a transaction");
    Console.WriteLine("5. See Balance");
    Console.WriteLine("6. View Blockchain");
    Console.WriteLine("Press 'q' to quit or any other key to continue...");
    var input = Console.ReadKey(true).KeyChar;
    switch (input)
    {
        case '1':
            if(wallet != null)
            {
                Console.WriteLine("You are already logged in with a wallet. Please log out first.");
                Console.ReadKey();
                break;
            }
            wallet = Wallet.Create();
            Console.WriteLine("New wallet created with Public Key: " + wallet.PublicKey);
            Console.WriteLine("Private Key: " + wallet.GetPrivateKey());
            Console.ReadKey();
            break;
        case '2':
            if (wallet != null)
            {
                Console.WriteLine("Current wallet is still active. Please log out first.");
                break;
            }
            Console.WriteLine("Enter the private key to export the wallet:");
            var privateKey = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(privateKey))
            {
                Console.WriteLine("Private key cannot be empty.");
                break;
            }
            try
            {
                wallet = Wallet.Export(privateKey);
                Console.WriteLine("Wallet exported successfully!");
                Console.WriteLine("Public Key: " + wallet.PublicKey);
                Console.WriteLine("Private Key: " + wallet.GetPrivateKey());
            } catch (Exception ex)
            {
                Console.WriteLine("Error exporting wallet: " + ex.Message);
            }
            Console.ReadKey();
            break;
        case '3':
            if(wallet == null)
            {
                Console.WriteLine("No wallet created. Please create a wallet first.");
                Console.ReadKey();
                break;
            }
            Console.WriteLine("Are you sure you want to log out from the current wallet? (y/n)");
            var confirmation = Console.ReadKey(true).KeyChar;
            if (confirmation == 'y' || confirmation == 'Y')
            {
                wallet = null;
                Console.WriteLine("Current wallet log out successfully.");
            }
            else
            {
                Console.WriteLine("Log out cancelled.");
            }
            Console.ReadKey();
            break;
        case '4':
            if (wallet == null)
            {
                Console.WriteLine("No wallet created. Please create a wallet first.");
                Console.ReadKey();
                break;
            }
            Console.WriteLine("Enter the recipient's address:");
            var toAddress = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(toAddress))
            {
                Console.WriteLine("Recipient address cannot be empty.");
                Console.ReadKey();
                break;
            }
            Console.WriteLine("Enter the amount of WiwokCoin to send:");
            if (!decimal.TryParse(Console.ReadLine()?.Trim(), out var amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a valid decimal number greater than zero.");
                Console.ReadKey();
                break;
            }
            var transaction = new Transaction(wallet.PublicKey, toAddress, amount);
            try
            {
                service.AddTransaction(transaction, wallet.GetPrivateKey());
                Console.WriteLine("Transaction added successfully!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding transaction: " + ex.Message);
                Console.ReadKey();
            }
            break;
        case '5':
            if (wallet == null)
            {
                Console.WriteLine("No wallet created. Please create a wallet first.");
                Console.ReadKey();
                break;
            }
            Console.WriteLine("Balance for wallet: " + service.GetBalance(wallet.PublicKey));
            Console.ReadKey();
            break;
        case '6':
            var chain = blockchain.GetChain();
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
            break;
    }
    if (input == 'q' || input == 'Q')
    {
        Console.WriteLine("Exiting the application.");
        break;
    }
}