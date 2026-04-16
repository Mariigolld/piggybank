using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    public class BankAccountsController : BaseApiController
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        /// <summary>
        /// Dohvata sve bankovne račune korisnika
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetUserAccounts()
        {
            try
            {
                var userId = GetUserId();
                var accounts = await _bankAccountService.GetUserAccountsAsync(userId);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dohvata bankovni račun po ID-u
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccountDto>> GetById(int id)
        {
            try
            {
                var userId = GetUserId();
                var account = await _bankAccountService.GetByIdAsync(id, userId);
                return Ok(account);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Kreira novi bankovni račun
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BankAccountDto>> Create([FromBody] CreateBankAccountDto dto)
        {
            try
            {
                var userId = GetUserId();
                var account = await _bankAccountService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Ažurira bankovni račun
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<BankAccountDto>> Update(int id, [FromBody] UpdateBankAccountDto dto)
        {
            try
            {
                var userId = GetUserId();
                var account = await _bankAccountService.UpdateAsync(id, dto, userId);
                return Ok(account);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Briše bankovni račun
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                await _bankAccountService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Toggles whether account spending is included in shared budget
        /// </summary>
        [HttpPatch("{id}/toggle-shared")]
        public async Task<ActionResult<BankAccountDto>> ToggleSharedBudget(int id)
        {
            try
            {
                var userId = GetUserId();
                var account = await _bankAccountService.ToggleSharedBudgetAsync(id, userId);
                return Ok(account);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>
        /// Dohvata ukupan balans svih računa korisnika
        /// </summary>
        [HttpGet("total-balance")]
        public async Task<ActionResult<decimal>> GetTotalBalance()
        {
            try
            {
                var userId = GetUserId();
                var balance = await _bankAccountService.GetTotalBalanceAsync(userId);
                return Ok(new { totalBalance = balance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}