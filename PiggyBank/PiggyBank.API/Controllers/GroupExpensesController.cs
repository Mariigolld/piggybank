using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    public class GroupExpensesController : BaseApiController
    {
        private readonly IGroupExpenseService _groupExpenseService;

        public GroupExpensesController(IGroupExpenseService groupExpenseService)
        {
            _groupExpenseService = groupExpenseService;
        }

        /// <summary>
        /// Gets all group expenses
        /// </summary>
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<GroupExpenseDto>>> GetGroupExpenses(int groupId)
        {
            try
            {
                var userId = GetUserId();
                var expenses = await _groupExpenseService.GetGroupExpensesAsync(groupId, userId);
                return Ok(expenses);
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
        /// Gets expense by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupExpenseDto>> GetById(int id)
        {
            try
            {
                var userId = GetUserId();
                var expense = await _groupExpenseService.GetByIdAsync(id, userId);
                return Ok(expense);
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
        /// Creates new group expense
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GroupExpenseDto>> Create([FromBody] CreateGroupExpenseDto dto)
        {
            try
            {
                var userId = GetUserId();
                var expense = await _groupExpenseService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates new group expense with selective split (selected members)
        /// </summary>
        [HttpPost("split")]
        public async Task<ActionResult<GroupExpenseDto>> CreateWithSelectiveSplit([FromBody] CreateGroupExpenseWithSplitDto dto)
        {
            try
            {
                var userId = GetUserId();
                var expense = await _groupExpenseService.CreateWithSelectiveSplitAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes group expense
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                await _groupExpenseService.DeleteAsync(id, userId);
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
        /// Marks expense share as paid
        /// </summary>
        [HttpPost("shares/{shareId}/mark-paid")]
        public async Task<ActionResult> MarkShareAsPaid(int shareId)
        {
            try
            {
                var userId = GetUserId();
                await _groupExpenseService.MarkShareAsPaidAsync(shareId, userId);
                return Ok(new { message = "Share marked as paid" });
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
    }
}