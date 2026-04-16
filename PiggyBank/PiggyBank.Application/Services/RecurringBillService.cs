using AutoMapper;
using PiggyBank.Core.Entities;
using PiggyBank.Core.Interfaces;
using PiggyBank.Application.DTOs;

namespace PiggyBank.Application.Services
{
    public class RecurringBillService : IRecurringBillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecurringBillService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecurringBillDto>> GetBySharedAccountAsync(int sharedAccountId, string userId)
        {
            await VerifySharedAccountAccessAsync(sharedAccountId, userId);
            var bills = await _unitOfWork.RecurringBills.GetBySharedAccountIdAsync(sharedAccountId);
            return bills.Select(MapBillToDto).ToList();
        }

        public async Task<RecurringBillDto> GetByIdAsync(int billId, string userId)
        {
            var bill = await _unitOfWork.RecurringBills.GetWithPaymentsAsync(billId)
                ?? throw new KeyNotFoundException("Recurring bill not found");
            await VerifySharedAccountAccessAsync(bill.SharedAccountId, userId);
            return MapBillToDto(bill);
        }

        public async Task<RecurringBillDto> CreateAsync(int sharedAccountId, CreateRecurringBillDto dto, string userId)
        {
            await VerifySharedAccountAccessAsync(sharedAccountId, userId);

            var bill = new RecurringBill
            {
                SharedAccountId = sharedAccountId,
                Name = dto.Name,
                Amount = dto.Amount,
                Frequency = dto.Frequency,
                PaidBy = dto.PaidBy,
                DayOfMonth = dto.DayOfMonth,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RecurringBills.AddAsync(bill);
            await _unitOfWork.SaveChangesAsync();

            // Auto-generate payments for the next 12 months
            var now = DateTime.UtcNow;
            var payments = new List<RecurringBillPayment>();
            for (int i = 0; i < 12; i++)
            {
                var date = now.AddMonths(i);
                payments.Add(new RecurringBillPayment
                {
                    RecurringBillId = bill.Id,
                    Month = date.Month,
                    Year = date.Year,
                    IsPaid = false
                });
            }

            foreach (var p in payments)
            {
                bill.Payments.Add(p);
            }
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.RecurringBills.GetWithPaymentsAsync(bill.Id);
            return MapBillToDto(created!);
        }

        public async Task<RecurringBillPaymentDto> MarkPaymentPaidAsync(int billId, int month, int year, string userId, MarkBillPaidDto dto)
        {
            var bill = await _unitOfWork.RecurringBills.GetWithPaymentsAsync(billId)
                ?? throw new KeyNotFoundException("Recurring bill not found");
            await VerifySharedAccountAccessAsync(bill.SharedAccountId, userId);

            var payment = bill.Payments.FirstOrDefault(p => p.Month == month && p.Year == year)
                ?? throw new KeyNotFoundException($"Payment for {month}/{year} not found");

            payment.IsPaid = true;
            payment.PaidByUserId = userId;
            payment.PaidAt = DateTime.UtcNow;
            payment.Note = dto.Note;
            await _unitOfWork.SaveChangesAsync();

            return MapPaymentToDto(payment);
        }

        public async Task<RecurringBillPaymentDto> MarkPaymentUnpaidAsync(int billId, int month, int year, string userId)
        {
            var bill = await _unitOfWork.RecurringBills.GetWithPaymentsAsync(billId)
                ?? throw new KeyNotFoundException("Recurring bill not found");
            await VerifySharedAccountAccessAsync(bill.SharedAccountId, userId);

            var payment = bill.Payments.FirstOrDefault(p => p.Month == month && p.Year == year)
                ?? throw new KeyNotFoundException($"Payment for {month}/{year} not found");

            payment.IsPaid = false;
            payment.PaidByUserId = null;
            payment.PaidAt = null;
            payment.Note = null;
            await _unitOfWork.SaveChangesAsync();

            return MapPaymentToDto(payment);
        }

        public async Task DeactivateAsync(int billId, string userId)
        {
            var bill = await _unitOfWork.RecurringBills.GetByIdAsync(billId)
                ?? throw new KeyNotFoundException("Recurring bill not found");
            await VerifySharedAccountAccessAsync(bill.SharedAccountId, userId);

            bill.IsActive = false;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReactivateAsync(int billId, string userId)
        {
            var bill = await _unitOfWork.RecurringBills.GetByIdAsync(billId)
                ?? throw new KeyNotFoundException("Recurring bill not found");
            await VerifySharedAccountAccessAsync(bill.SharedAccountId, userId);

            bill.IsActive = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int billId, string userId)
        {
            var bill = await _unitOfWork.RecurringBills.GetByIdAsync(billId)
                ?? throw new KeyNotFoundException("Recurring bill not found");
            await VerifySharedAccountAccessAsync(bill.SharedAccountId, userId);

            await _unitOfWork.RecurringBills.DeleteAsync(bill);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task VerifySharedAccountAccessAsync(int sharedAccountId, string userId)
        {
            var account = await _unitOfWork.SharedAccounts.GetByIdAsync(sharedAccountId)
                ?? throw new KeyNotFoundException("Shared account not found");
            if (account.User1Id != userId && account.User2Id != userId)
                throw new UnauthorizedAccessException("You do not have access to this shared account");
        }

        private static RecurringBillDto MapBillToDto(RecurringBill bill)
        {
            return new RecurringBillDto
            {
                Id = bill.Id,
                SharedAccountId = bill.SharedAccountId,
                Name = bill.Name,
                Amount = bill.Amount,
                Frequency = bill.Frequency,
                PaidBy = bill.PaidBy,
                DayOfMonth = bill.DayOfMonth,
                IsActive = bill.IsActive,
                CreatedAt = bill.CreatedAt,
                Payments = bill.Payments.OrderBy(p => p.Year).ThenBy(p => p.Month).Select(MapPaymentToDto).ToList()
            };
        }

        private static RecurringBillPaymentDto MapPaymentToDto(RecurringBillPayment payment)
        {
            return new RecurringBillPaymentDto
            {
                Id = payment.Id,
                RecurringBillId = payment.RecurringBillId,
                Month = payment.Month,
                Year = payment.Year,
                IsPaid = payment.IsPaid,
                PaidByUser = payment.PaidByUser == null ? null : new UserInfoDto
                {
                    Id = payment.PaidByUser.Id,
                    Email = payment.PaidByUser.Email ?? string.Empty,
                    FirstName = payment.PaidByUser.FirstName,
                    LastName = payment.PaidByUser.LastName
                },
                PaidAt = payment.PaidAt,
                Note = payment.Note
            };
        }
    }
}
