using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    [Route("api/SharedAccounts/{sharedAccountId}/SavingsGoals")]
    public class SavingsGoalsController : BaseApiController
    {
        private readonly ISavingsGoalService _savingsGoalService;

        public SavingsGoalsController(ISavingsGoalService savingsGoalService)
        {
            _savingsGoalService = savingsGoalService;
        }

        /// <summary>Get all savings goals for a shared account</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavingsGoalDto>>> GetAll(int sharedAccountId, [FromQuery] bool includeArchived = false)
        {
            try
            {
                var userId = GetUserId();
                var goals = await _savingsGoalService.GetBySharedAccountAsync(sharedAccountId, userId, includeArchived);
                return Ok(goals);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Get a specific savings goal</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SavingsGoalDto>> GetById(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                var goal = await _savingsGoalService.GetByIdAsync(id, userId);
                return Ok(goal);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Create a new savings goal</summary>
        [HttpPost]
        public async Task<ActionResult<SavingsGoalDto>> Create(int sharedAccountId, [FromBody] CreateSavingsGoalDto dto)
        {
            try
            {
                var userId = GetUserId();
                var goal = await _savingsGoalService.CreateAsync(sharedAccountId, dto, userId);
                return CreatedAtAction(nameof(GetById), new { sharedAccountId, id = goal.Id }, goal);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Add a contribution to a savings goal</summary>
        [HttpPost("{id}/contributions")]
        public async Task<ActionResult<SavingsGoalDto>> AddContribution(int sharedAccountId, int id, [FromBody] AddContributionDto dto)
        {
            try
            {
                var userId = GetUserId();
                var goal = await _savingsGoalService.AddContributionAsync(id, dto, userId);
                return Ok(goal);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Archive a savings goal</summary>
        [HttpPatch("{id}/archive")]
        public async Task<ActionResult> Archive(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                await _savingsGoalService.ArchiveAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Delete a savings goal (only if no contributions)</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                await _savingsGoalService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}
