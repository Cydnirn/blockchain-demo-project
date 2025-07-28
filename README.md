# Blockchain Demo Project

This is a simple blockchain demo project that demonstrates the basic concepts of blockchain technology, including blocks, transactions, and consensus mechanisms.

This is just a demo project and is not intended for production use. It is meant to be a learning tool for understanding how blockchains work.

This project is built using C# and .NET Core, and it can be run on any platform that supports .NET Core.

This project is created as part of a submission for "Programming Final Assignment" to implement OOP pillars for the course "Programming Fundamentals" at the University of Amikom Yogyakarta, Indonesia.

## Features
- Create and manage blocks
- Add transactions to blocks
- Implement a simple proof-of-work consensus mechanism
- Validate the blockchain
- Display the blockchain
- Basic command-line interface for interaction

## Requirements
- .Net Core 6.0 or later
- Or Docker to run the application in a container

## Getting Started
### Clone the Repository
```bash
git clone https://github.com/Cydnirn/blockchain-demo-project.git
```

### Build the Project
Navigate to the project directory and build the project using the following command:
```bash
dotnet build
```

Or open the project in Visual Studio, JetBrains Rider, or any other compatible IDE and build it from there.

### Run the Application
You can run the application using the following command:
```bash
dotnet run
```
Or if you prefer to run it in a Docker container, you can use the following command:
```bash
docker build -t blockchain-demo .
docker run blockchain-demo
```

Or run the application directly from Visual Studio or JetBrains Rider.

## Future Enhancements
- Implement a more sophisticated consensus mechanism (e.g., Proof of Stake)
- Implement a peer-to-peer network for decentralized blockchain
- Add more features like smart contracts, token creation, etc.
- Implement unit tests for better code coverage