# BudgetWise — Monthly Budget Planner

An ASP.NET Core MVC project using C#, Entity Framework Core, and MySQL.

## Features
- Monthly income can be added or updated by month/year.
- Expense CRUD: create, read, update, and delete.
- Custom categories with create, rename, and delete.
- Month/year and category filters.
- Dashboard summary cards for income, expenses, and balance.
- Server-side model validation and client-side validation support.
- TempData success/error messages.
- Responsive Bootstrap UI.
- No charts, graphs, login, or authentication.

## Requirements
- .NET 10 SDK
- MySQL Server running locally
- MySQL Workbench (for inspecting the database; optional for app runtime)

## Setup

1. Install MySQL Server and ensure it is running.
2. Open `appsettings.json` and replace `CHANGE_ME` with your MySQL root password.
3. Open a terminal in the project folder and run:

```bash
dotnet restore
dotnet tool install --global dotnet-ef --version 10.0.2
```

If `dotnet-ef` is already installed, update it:

```bash
dotnet tool update --global dotnet-ef --version 10.0.2
```

4. Create the database schema with EF Core migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

The configured connection uses `budgetwise_db`. The provider creates the database if the MySQL account has permission to do so. If not, create it manually in Workbench:

```sql
CREATE DATABASE budgetwise_db;
```

5. Start the app:

```bash
dotnet run
```

Open the local URL printed by the terminal.

## Database tables
- `Categories`: custom category names.
- `Expenses`: title, amount, date, category foreign key, optional description.
- `MonthlyBudgets`: one income record per month/year.

## Notes
- Category names are unique (case-insensitive duplicate checks are performed by the app).
- A category cannot be deleted while expenses reference it.
- Change the database credentials locally; never commit real passwords to a public repository.
- This sample uses Oracle's `MySql.EntityFrameworkCore` provider. If NuGet reports a package-version incompatibility in your environment, install the provider version compatible with your installed EF Core version rather than mixing major versions.
