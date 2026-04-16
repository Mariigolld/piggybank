# PiggyBank

A personal finance and group expense management web application built with **.NET 8** and **Angular 18**.

## Features

### Personal Finance
- **Bank Accounts** — create multiple accounts with different currencies (RSD, EUR, USD), track balances, and toggle whether each account counts toward the shared budget
- **Transactions** — record income and expenses, assign categories, filter by date range
- **Categories** — create custom categories with emoji icons
- **Dashboard** — activity chart (daily income vs expenses), expenses by category breakdown, account balance distribution donut, net this month summary

### Partner (Shared Account)
- **Shared Account** — link two users via invite code for a joint financial view
- **Savings Goals** — set a target amount and date, track contributions from both partners, archive completed goals
- **Recurring Bills** — define monthly/weekly/yearly bills, specify who pays (User 1, User 2, alternating, 50/50), mark payments as paid, pause/resume bills, view payment history
- **Shared Budget** — set a monthly budget by category, automatically tracks spending from accounts marked as shared

### Groups
- **Group Expenses** — create groups with multiple members, add expenses with equal or custom splits, view each member's balance
- **Settlements** — record payments between members to settle debts, mark individual expense shares as paid



## How to Run

### Prerequisites
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with ASP.NET and web development workload) — includes .NET 8 SDK and SQL Server LocalDB
- [Node.js 18+](https://nodejs.org/)

### Install frontend dependencies (first time only)

```bash
cd piggybank-angular
npm install
```

### Run

1. Open `PiggyBank/PiggyBank.sln` in Visual Studio
2. Press **F5**

This starts both the backend API (`http://localhost:5235`) and the Angular frontend (`http://localhost:4200`). The database is created and migrations applied automatically on first run.


## Project Structure

```
PiggyBank2026/
├── PiggyBank/                        # .NET solution
│   ├── PiggyBank.Core/               # Domain entities & repository interfaces
│   ├── PiggyBank.Application/        # Business logic, services, DTOs, AutoMapper
│   ├── PiggyBank.Infrastructure/     # EF Core DbContext, repositories, migrations
│   └── PiggyBank.API/                # ASP.NET Core controllers, JWT setup
└── piggybank-angular/                # Angular 18 frontend
    └── src/app/
        ├── pages/                    # Dashboard, Partner, Groups, Login, Register
        ├── services/                 # HTTP services per domain
        ├── models/                   # TypeScript interfaces & enums
        └── shared/                   # Sidebar component
```
