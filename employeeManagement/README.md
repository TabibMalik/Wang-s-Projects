# Employee Management Console App

This is a simple C# console application to manage employees:

- Add new employees
- Update employee details
- Delete employees
- Search by ID or name
- Display all employees
- Payroll menu (list, simulate pay, adjust salary)
- Vacation menu (add/view vacation days)

Run with the .NET SDK:

```powershell
cd 'd:\C#\employeeManagement'
dotnet new console -n EmployeeManagement -o EmployeeManagement --force
Copy-Item .\Program.cs -Destination .\EmployeeManagement\Program.cs -Force
cd .\EmployeeManagement
dotnet run
```
