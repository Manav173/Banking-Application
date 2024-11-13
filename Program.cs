using System;
using System.Collections.Generic;

class Program
{
    static List<User> users = new List<User>();
    static User loggedInUser;

    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Banking Application ===");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    LoginUser();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to try again.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void RegisterUser()
    {
        Console.Clear();
        Console.WriteLine("=== Register ===");
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        if (users.Exists(u => u.Username == username))
        {
            Console.WriteLine("Username already exists. Press Enter to return.");
        }
        else
        {
            users.Add(new User(username, password));
            Console.WriteLine("Registration successful! Press Enter to return.");
        }
        Console.ReadLine();
    }

    static void LoginUser()
    {
        Console.Clear();
        Console.WriteLine("=== Login ===");
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        var user = users.Find(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            loggedInUser = user;
            Console.WriteLine("Login successful! Press Enter to continue.");
            Console.ReadLine();
            UserMenu();
        }
        else
        {
            Console.WriteLine("Invalid credentials. Press Enter to return.");
            Console.ReadLine();
        }
    }

    static void UserMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== User Menu ===");
            Console.WriteLine("1. Open Account");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Generate Statement");
            Console.WriteLine("5. Check Balance");
            Console.WriteLine("6. Logout");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    loggedInUser.OpenAccount();
                    break;
                case "2":
                    PerformAction("Deposit");
                    break;
                case "3":
                    PerformAction("Withdraw");
                    break;
                case "4":
                    PerformAction("GenerateStatement");
                    break;
                case "5":
                    PerformAction("CheckBalance");
                    break;
                case "6":
                    loggedInUser = null;
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to try again.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void PerformAction(string actionType)
    {
        if (loggedInUser.Accounts.Count == 0)
        {
            Console.WriteLine("No accounts found. Please open an account first. Press Enter to return.");
            Console.ReadLine();
            return;
        }

        Console.Clear();
        Console.WriteLine($"=== {actionType} ===");
        Console.WriteLine("Select an account:");

        for (int i = 0; i < loggedInUser.Accounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Account Number : {loggedInUser.Accounts[i].AccountNumber} ({loggedInUser.Accounts[i].Type})");
        }

        Console.Write("Enter your choice: ");
        if (int.TryParse(Console.ReadLine(), out int accountChoice) &&
            accountChoice > 0 &&
            accountChoice <= loggedInUser.Accounts.Count)
        {
            var selectedAccount = loggedInUser.Accounts[accountChoice - 1];

            switch (actionType)
            {
                case "Deposit":
                    selectedAccount.PerformTransaction("Deposit");
                    break;
                case "Withdraw":
                    selectedAccount.PerformTransaction("Withdraw");
                    break;
                case "GenerateStatement":
                    selectedAccount.PrintStatement();
                    Console.WriteLine("Press Enter to return.");
                    Console.ReadLine();
                    break;
                case "CheckBalance":
                    Console.WriteLine($"Current Balance: {selectedAccount.Balance:C}");
                    Console.WriteLine("Press Enter to return.");
                    Console.ReadLine();
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid selection. Press Enter to return.");
            Console.ReadLine();
        }
    }
}

class User
{
    public string Username { get; }
    public string Password { get; }
    public List<Account> Accounts { get; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
        Accounts = new List<Account>();
    }

    public void OpenAccount()
    {
        Console.Clear();
        Console.WriteLine("=== Open Account ===");
        Console.Write("Enter Account Holder Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Account Type (Savings/Checking): ");
        string type = Console.ReadLine();
        Console.Write("Enter Initial Deposit: ");

        if (decimal.TryParse(Console.ReadLine(), out decimal initialDeposit) && initialDeposit > 0)
        {
            var account = new Account(name, type, initialDeposit);
            Accounts.Add(account);
            Console.WriteLine($"Account created successfully! Account Number: {account.AccountNumber}");
        }
        else
        {
            Console.WriteLine("Invalid deposit amount. Account not created.");
        }
        Console.WriteLine("Press Enter to return.");
        Console.ReadLine();
    }
}

class Account
{
    private static int AccountCounter = 1000;
    public string AccountHolder { get; }
    public string AccountNumber { get; }
    public string Type { get; }
    public decimal Balance { get; private set; }
    public List<Transaction> Transactions { get; }

    public Account(string holder, string type, decimal initialDeposit)
    {
        AccountHolder = holder;
        Type = type;
        Balance = initialDeposit;
        AccountNumber = (AccountCounter++).ToString();
        Transactions = new List<Transaction>();
        LogTransaction("Initial Deposit", initialDeposit);
    }

    public void PerformTransaction(string transactionType)
    {
        Console.Write($"Enter Amount for {transactionType}: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
        {
            if (transactionType == "Deposit")
            {
                Balance += amount;
                LogTransaction(transactionType, amount);
                Console.WriteLine("Deposit successful!");
            }
            else if (transactionType == "Withdraw")
            {
                if (Balance >= amount)
                {
                    Balance -= amount;
                    LogTransaction(transactionType, amount);
                    Console.WriteLine("Withdrawal successful!");
                }
                else
                {
                    Console.WriteLine("Insufficient funds.");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid amount.");
        }
        Console.WriteLine("Press Enter to return.");
        Console.ReadLine();
    }

    public void PrintStatement()
    {
        Console.WriteLine($"=== Statement for Account {AccountNumber} ===");
        Console.WriteLine($"Account Holder: {AccountHolder}");
        Console.WriteLine($"Account Type: {Type}");
        Console.WriteLine($"Balance: {Balance:C}");
        Console.WriteLine("Transactions:");
        foreach (var t in Transactions)
        {
            Console.WriteLine($"{t.Date}: {t.Type} - {t.Amount:C}");
        }
    }

    private void LogTransaction(string type, decimal amount)
    {
        Transactions.Add(new Transaction(type, amount));
    }
}

class Transaction
{
    public string Type { get; }
    public decimal Amount { get; }
    public DateTime Date { get; }

    public Transaction(string type, decimal amount)
    {
        Type = type;
        Amount = amount;
        Date = DateTime.Now;
    }
}
