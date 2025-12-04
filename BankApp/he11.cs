using System;
using System.Collections.Generic;

class Account
{
    public int AccountNumber { get; }
    public string HolderName { get; }
    public decimal Balance { get; private set; }

    public Account(int number, string holder, decimal initialDeposit)
    {
        AccountNumber = number;
        HolderName = holder;
        Balance = initialDeposit;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Deposit amount must be positive.");
        Balance += amount;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Withdrawal amount must be positive.");
        if (amount > Balance) return false;
        Balance -= amount;
        return true;
    }

    public void Display()
    {
        Console.WriteLine("-----------------------------");
        Console.WriteLine($"Account Number: {AccountNumber}");
        Console.WriteLine($"Holder Name:    {HolderName}");
        Console.WriteLine($"Balance:        {Balance:C}");
        Console.WriteLine("-----------------------------");
    }
}

class Bank
{
    private readonly Dictionary<int, Account> _accounts = new Dictionary<int, Account>();
    private int _nextAccountNumber = 1001;

    public Account CreateAccount(string holderName, decimal initialDeposit)
    {
        var acc = new Account(_nextAccountNumber++, holderName, initialDeposit);
        _accounts.Add(acc.AccountNumber, acc);
        return acc;
    }

    public bool TryGetAccount(int number, out Account account)
    {
        return _accounts.TryGetValue(number, out account);
    }

    public IEnumerable<Account> GetAllAccounts() => _accounts.Values;
}

class Program
{
    static void Main()
    {
        var bank = new Bank();
        bool running = true;

        while (running)
        {
            ShowMenu();
            Console.Write("Select an option: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1": // Create account
                    CreateAccount(bank);
                    break;
                case "2": // Deposit
                    DepositToAccount(bank);
                    break;
                case "3": // Withdraw
                    WithdrawFromAccount(bank);
                    break;
                case "4": // Check balance
                    CheckBalance(bank);
                    break;
                case "5": // Display details
                    DisplayAccountDetails(bank);
                    break;
                case "6": // List accounts
                    ListAccounts(bank);
                    break;
                case "7": // Exit
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please enter a number from the menu.");
                    break;
            }
        }

        Console.WriteLine("Goodbye.");
    }

    static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Simple Banking Application");
        Console.WriteLine("1. Create account");
        Console.WriteLine("2. Deposit");
        Console.WriteLine("3. Withdraw");
        Console.WriteLine("4. Check balance");
        Console.WriteLine("5. Display account details");
        Console.WriteLine("6. List accounts");
        Console.WriteLine("7. Exit (or press 0)");
    }

    static void CreateAccount(Bank bank)
    {
        Console.Write("Enter holder name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Holder name cannot be empty.");
            return;
        }

        Console.Write("Initial deposit (or leave empty for 0): ");
        if (!TryReadDecimal(Console.ReadLine(), out decimal initial))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        try
        {
            var acc = bank.CreateAccount(name, initial);
            Console.WriteLine($"Account created successfully. Account Number: {acc.AccountNumber}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not create account: {ex.Message}");
        }
    }

    static void DepositToAccount(Bank bank)
    {
        if (!ReadAccountNumber("Enter account number: ", bank, out var acc)) return;

        Console.Write("Enter deposit amount: ");
        if (!TryReadDecimal(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        try
        {
            acc.Deposit(amount);
            Console.WriteLine($"Deposited {amount:C} to account {acc.AccountNumber}. New balance: {acc.Balance:C}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deposit failed: {ex.Message}");
        }
    }

    static void WithdrawFromAccount(Bank bank)
    {
        if (!ReadAccountNumber("Enter account number: ", bank, out var acc)) return;

        Console.Write("Enter withdrawal amount: ");
        if (!TryReadDecimal(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        try
        {
            var ok = acc.Withdraw(amount);
            if (ok) Console.WriteLine($"Withdrew {amount:C}. New balance: {acc.Balance:C}");
            else Console.WriteLine("Insufficient funds.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Withdrawal failed: {ex.Message}");
        }
    }

    static void CheckBalance(Bank bank)
    {
        if (!ReadAccountNumber("Enter account number: ", bank, out var acc)) return;
        Console.WriteLine($"Account {acc.AccountNumber} balance: {acc.Balance:C}");
    }

    static void DisplayAccountDetails(Bank bank)
    {
        if (!ReadAccountNumber("Enter account number: ", bank, out var acc)) return;
        acc.Display();
    }

    static void ListAccounts(Bank bank)
    {
        Console.WriteLine("Accounts:");
        foreach (var a in bank.GetAllAccounts())
        {
            Console.WriteLine($"- {a.AccountNumber}: {a.HolderName} ({a.Balance:C})");
        }
    }

    static bool ReadAccountNumber(string prompt, Bank bank, out Account acc)
    {
        acc = null;
        Console.Write(prompt);
        var input = Console.ReadLine();
        if (!int.TryParse(input, out int num))
        {
            Console.WriteLine("Invalid account number.");
            return false;
        }

        if (!bank.TryGetAccount(num, out acc))
        {
            Console.WriteLine("Account not found.");
            return false;
        }

        return true;
    }

    static bool TryReadDecimal(string input, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(input)) { value = 0m; return true; }
        return decimal.TryParse(input, out value) && value >= 0m;
    }
}

