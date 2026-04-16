using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    [Route("api/SharedAccounts/{sharedAccountId}/SharedBudgets")]
    public class SharedBudgetsController : BaseApiController
    {
        private readonly ISharedBudgetService _sharedBudgetService;

        public SharedBudgetsController(ISharedBudgetService sharedBudgetService)
        {
            _sharedBudgetService = sharedBudgetService;
        }

        /// <summary>Get all budgets for a shared account</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SharedBudgetDto>>> GetAll(int sharedAccountId)
        {
            try
            {
                var userId = GetUserId();
                var budgets = await _sharedBudgetService.GetBySharedAccountAsync(sharedAccountId, userId);
                return Ok(budgets);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Get budget for a specific month/year</summary>
        [HttpGet("{year}/{month}")]
        public async Task<ActionResult<SharedBudgetDto>> GetByMonthYear(int sharedAccountId, int year, int month)
        {
            try
            {
                var userId = GetUserId();
                var budget = await _sharedBudgetService.GetByMonthYearAsync(sharedAccountId, month, year, userId);
                if (budget == null) return NotFound(new { message = "No budget found for this period" });
                return Ok(budget);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Create or update a budget for a month/year</summary>
        [HttpPost]
        public async Task<ActionResult<SharedBudgetDto>> CreateOrUpdate(int sharedAccountId, [FromBody] CreateSharedBudgetDto dto)
        {
            try
            {
                var userId = GetUserId();
                var budget = await _sharedBudgetService.CreateOrUpdateAsync(sharedAccountId, dto, userId);
                return Ok(budget);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Delete a budget</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                await _sharedBudgetService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}
