// See https://aka.ms/new-console-template for more information
using System;
using Blockchain_Demo_Project;

Console.WriteLine("Blockchain Demo Project");
var wallet = Wallet.Create();

Console.WriteLine("Wallet Public Key: " + wallet.PublicKey);
Console.WriteLine("Wallet Private Key: " + wallet.PrivateKey);