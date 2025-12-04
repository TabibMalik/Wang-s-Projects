using System;
using System.Collections.Generic;
using System.Linq;

class Employee
{
	public int Id { get; }
	public string Name { get; set; }
	public string Position { get; set; }
	public decimal Salary { get; set; }
	public int VacationDays { get; set; }

	public Employee(int id, string name, string position, decimal salary)
	{
		Id = id;
		Name = name;
		Position = position;
		Salary = salary;
		VacationDays = 0;
	}

	public void Display()
	{
		Console.WriteLine("----------------------------------------");
		Console.WriteLine($"ID: {Id}");
		Console.WriteLine($"Name: {Name}");
		Console.WriteLine($"Position: {Position}");
		Console.WriteLine($"Salary: {Salary:C}");
		Console.WriteLine($"Vacation Days: {VacationDays}");
		Console.WriteLine("----------------------------------------");
	}
}

class EmployeeManager
{
	private readonly Dictionary<int, Employee> _employees = new Dictionary<int, Employee>();
	private int _nextId = 1;

	public Employee AddEmployee(string name, string position, decimal salary)
	{
		var e = new Employee(_nextId++, name, position, salary);
		_employees.Add(e.Id, e);
		return e;
	}

	public bool UpdateEmployee(int id, string name, string position, decimal salary)
	{
		if (!_employees.TryGetValue(id, out var e)) return false;
		e.Name = name;
		e.Position = position;
		e.Salary = salary;
		return true;
	}

	public bool DeleteEmployee(int id)
	{
		return _employees.Remove(id);
	}

	public Employee GetById(int id)
	{
		_employees.TryGetValue(id, out var e);
		return e;
	}

	public IEnumerable<Employee> SearchByName(string query)
	{
		return _employees.Values.Where(e => e.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
	}

	public IEnumerable<Employee> GetAll() => _employees.Values.OrderBy(e => e.Id);

	public void AddVacationDays(int id, int days)
	{
		if (_employees.TryGetValue(id, out var e)) e.VacationDays += days;
	}
}

class Program
{
	static void Main()
	{
		var mgr = new EmployeeManager();
		bool running = true;

		while (running)
		{
			ShowMainMenu();
			Console.Write("Select an option: ");
			var opt = Console.ReadLine()?.Trim();
			switch (opt)
			{
				case "1": AddNew(mgr); break;
				case "2": UpdateExisting(mgr); break;
				case "3": DeleteEmployee(mgr); break;
				case "4": SearchMenu(mgr); break;
				case "5": DisplayAll(mgr); break;
				case "6": PayrollMenu(mgr); break;
				case "7": VacationMenu(mgr); break;
				case "0": running = false; break;
				default: Console.WriteLine("Invalid option."); break;
			}
		}

		Console.WriteLine("Exiting Employee Management System.");
	}

	static void ShowMainMenu()
	{
		Console.WriteLine();
		Console.WriteLine("Employee Management System");
		Console.WriteLine("1. Add new employee");
		Console.WriteLine("2. Update employee details");
		Console.WriteLine("3. Delete employee");
		Console.WriteLine("4. Search by ID or name");
		Console.WriteLine("5. Display all employees");
		Console.WriteLine("6. Payroll menu");
		Console.WriteLine("7. Vacation menu");
		Console.WriteLine("0. Exit");
	}

	static void AddNew(EmployeeManager mgr)
	{
		Console.Write("Name: ");
		var name = Console.ReadLine()?.Trim();
		Console.Write("Position: ");
		var pos = Console.ReadLine()?.Trim();
		Console.Write("Salary: ");
		if (!TryReadDecimal(Console.ReadLine(), out decimal sal)) { Console.WriteLine("Invalid salary."); return; }

		var e = mgr.AddEmployee(name, pos, sal);
		Console.WriteLine($"Added employee with ID {e.Id}.");
	}

	static void UpdateExisting(EmployeeManager mgr)
	{
		Console.Write("Employee ID to update: ");
		if (!TryReadInt(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid ID."); return; }
		var e = mgr.GetById(id);
		if (e == null) { Console.WriteLine("Employee not found."); return; }

		Console.Write($"Name ({e.Name}): ");
		var name = Console.ReadLine(); if (string.IsNullOrWhiteSpace(name)) name = e.Name;
		Console.Write($"Position ({e.Position}): ");
		var pos = Console.ReadLine(); if (string.IsNullOrWhiteSpace(pos)) pos = e.Position;
		Console.Write($"Salary ({e.Salary}): ");
		var salIn = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(salIn) && TryReadDecimal(salIn, out decimal nsal)) e.Salary = nsal;

		mgr.UpdateEmployee(id, name, pos, e.Salary);
		Console.WriteLine("Employee updated.");
	}

	static void DeleteEmployee(EmployeeManager mgr)
	{
		Console.Write("Employee ID to delete: ");
		if (!TryReadInt(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid ID."); return; }
		if (mgr.DeleteEmployee(id)) Console.WriteLine("Deleted."); else Console.WriteLine("Not found.");
	}

	static void SearchMenu(EmployeeManager mgr)
	{
		Console.WriteLine("a) Search by ID");
		Console.WriteLine("b) Search by name");
		Console.Write("Choice: ");
		var c = Console.ReadLine()?.Trim()?.ToLower();
		if (c == "a")
		{
			Console.Write("ID: "); if (!TryReadInt(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid ID."); return; }
			var e = mgr.GetById(id); if (e == null) { Console.WriteLine("Not found."); return; } e.Display();
		}
		else if (c == "b")
		{
			Console.Write("Name query: "); var q = Console.ReadLine()?.Trim();
			var matches = mgr.SearchByName(q).ToList();
			if (!matches.Any()) { Console.WriteLine("No matches."); return; }
			foreach (var m in matches) m.Display();
		}
		else Console.WriteLine("Invalid choice.");
	}

	static void DisplayAll(EmployeeManager mgr)
	{
		var all = mgr.GetAll().ToList();
		if (!all.Any()) { Console.WriteLine("No employees."); return; }
		foreach (var e in all) e.Display();
	}

	static void PayrollMenu(EmployeeManager mgr)
	{
		Console.WriteLine("Payroll Menu");
		Console.WriteLine("1. List payroll (all employees and salaries)");
		Console.WriteLine("2. Pay an employee (simulate)");
		Console.WriteLine("3. Adjust salary");
		Console.Write("Choice: ");
		var c = Console.ReadLine()?.Trim();
		switch (c)
		{
			case "1":
				foreach (var e in mgr.GetAll()) Console.WriteLine($"{e.Id}: {e.Name} - {e.Salary:C}");
				break;
			case "2":
				Console.Write("Employee ID to pay: "); if (!TryReadInt(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid ID."); return; }
				var emp = mgr.GetById(id); if (emp == null) { Console.WriteLine("Not found."); return; }
				Console.WriteLine($"Paid {emp.Name} {emp.Salary:C} (simulated).");
				break;
			case "3":
				Console.Write("Employee ID to adjust: "); if (!TryReadInt(Console.ReadLine(), out int aid)) { Console.WriteLine("Invalid ID."); return; }
				var ent = mgr.GetById(aid); if (ent == null) { Console.WriteLine("Not found."); return; }
				Console.Write($"Current salary {ent.Salary:C}. New salary: "); if (!TryReadDecimal(Console.ReadLine(), out decimal ns)) { Console.WriteLine("Invalid salary."); return; }
				ent.Salary = ns; Console.WriteLine("Salary updated.");
				break;
			default: Console.WriteLine("Invalid choice."); break;
		}
	}

	static void VacationMenu(EmployeeManager mgr)
	{
		Console.WriteLine("Vacation Menu");
		Console.WriteLine("1. Add vacation days to employee");
		Console.WriteLine("2. View employee vacation days");
		Console.Write("Choice: ");
		var c = Console.ReadLine()?.Trim();
		switch (c)
		{
			case "1":
				Console.Write("Employee ID: "); if (!TryReadInt(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid ID."); return; }
				Console.Write("Days to add: "); if (!TryReadInt(Console.ReadLine(), out int days)) { Console.WriteLine("Invalid number."); return; }
				mgr.AddVacationDays(id, days); Console.WriteLine("Added (if employee exists).");
				break;
			case "2":
				Console.Write("Employee ID: "); if (!TryReadInt(Console.ReadLine(), out int vid)) { Console.WriteLine("Invalid ID."); return; }
				var e = mgr.GetById(vid); if (e == null) { Console.WriteLine("Not found."); return; } Console.WriteLine($"{e.Name} has {e.VacationDays} vacation days.");
				break;
			default: Console.WriteLine("Invalid choice."); break;
		}
	}

	static bool TryReadDecimal(string input, out decimal value)
	{
		value = 0m;
		if (string.IsNullOrWhiteSpace(input)) return false;
		return decimal.TryParse(input, out value);
	}

	static bool TryReadInt(string input, out int value)
	{
		value = 0;
		if (string.IsNullOrWhiteSpace(input)) return false;
		return int.TryParse(input, out value);
	}
}
