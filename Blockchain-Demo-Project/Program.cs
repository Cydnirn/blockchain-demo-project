// See https://aka.ms/new-console-template for more information
using Blockchain_Demo_Project.Services;
using Blockchain_Demo_Project.Blockchain;
using Blockchain_Demo_Project.Interfaces;

var blockchain = new Mainchain();
var testChain = new TestChain();
var chains = new Chains
{
    Mainnet = blockchain,
    Testnet = testChain
};
var service = BlockchainService.Create(chains.Mainnet);
var consoleService = ConsoleService.Create(service, chains);

while (true)
{
    //Initiate the first wallet if it exists
    consoleService.InitializeChains();
    Console.Clear();
    Console.WriteLine("Blockchain Demo Project");
    Console.WriteLine("Current Blockchain: " + consoleService.GetChainName());
    Console.WriteLine("Current Wallet: " + (consoleService.GetWallet() != null ? consoleService.GetWallet().PublicKey : "No wallet created"));
    Console.WriteLine("1. Create a new wallet");
    Console.WriteLine("2. Export an existing wallet");
    Console.WriteLine("3. Log out from the current wallet");
    Console.WriteLine("4. Send a transaction");
    Console.WriteLine("5. See Balance");
    Console.WriteLine("6. View Blockchain");
    Console.WriteLine("7. Select a different blockchain");
    Console.WriteLine("8. Validate the current blockchain");
    Console.WriteLine("Press 'q' to quit or any other key to continue...");
    var input = Console.ReadKey(true).KeyChar;
    switch (input)
    {
        case '1':
            consoleService.CreateWallet();
            break;
        case '2':
            consoleService.ExportWallet();
            break;
        case '3':
            consoleService.LogOutWallet();
            break;
        case '4':
            consoleService.SendTx();
            break;
        case '5':
            consoleService.CheckBalance();
            break;
        case '6':
            consoleService.GetChain();
            break;
        case '7':
            consoleService.SelectChain();
            break;
        case '8':
            consoleService.ValidateChain();
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            Console.ReadKey();
            continue;
    }
    if (input == 'q' || input == 'Q')
    {
        Console.WriteLine("Exiting the application.");
        break;
    }
}