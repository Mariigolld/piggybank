using Microsoft.EntityFrameworkCore.Storage;
using PiggyBank.Core.Interfaces;
using PiggyBank.Infrastructure.Data;

namespace PiggyBank.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            BankAccounts = new BankAccountRepository(_context);
            Categories = new CategoryRepository(_context);
            Transactions = new TransactionRepository(_context);
            SharedAccounts = new SharedAccountRepository(_context);
            SavingsGoals = new SavingsGoalRepository(_context);
            RecurringBills = new RecurringBillRepository(_context);
            SharedBudgets = new SharedBudgetRepository(_context);
            Groups = new GroupRepository(_context);
            GroupExpenses = new GroupExpenseRepository(_context);
        }

        public IBankAccountRepository BankAccounts { get; }
        public ICategoryRepository Categories { get; }
        public ITransactionRepository Transactions { get; }
        public ISharedAccountRepository SharedAccounts { get; }
        public ISavingsGoalRepository SavingsGoals { get; }
        public IRecurringBillRepository RecurringBills { get; }
        public ISharedBudgetRepository SharedBudgets { get; }
        public IGroupRepository Groups { get; }
        public IGroupExpenseRepository GroupExpenses { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}