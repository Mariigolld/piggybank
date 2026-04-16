using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    public class TransactionsController : BaseApiController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Dohvata transakcije sa paginacijom
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TransactionDto>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageSize > 100) pageSize = 100;

                var userId = GetUserId();
                var result = await _transactionService.GetPagedAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dohvata transakciju po ID-u
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetById(int id)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.GetByIdAsync(id, userId);
                return Ok(transaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Kreira novu transakciju
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionDto dto)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _transactionService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Briše transakciju
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                await _transactionService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dohvata transakcije u određenom periodu
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var userId = GetUserId();
                var transactions = await _transactionService.GetByDateRangeAsync(userId, from, to);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}