using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    public class SharedAccountsController : BaseApiController
    {
        private readonly ISharedAccountService _sharedAccountService;

        public SharedAccountsController(ISharedAccountService sharedAccountService)
        {
            _sharedAccountService = sharedAccountService;
        }

        /// <summary>
        /// Get all active shared accounts for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SharedAccountDto>>> GetUserSharedAccounts()
        {
            try
            {
                var userId = GetUserId();
                var sharedAccounts = await _sharedAccountService.GetUserSharedAccountsAsync(userId);
                return Ok(sharedAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get shared accounts waiting for a partner
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<SharedAccountDto>>> GetPendingInvitations()
        {
            try
            {
                var userId = GetUserId();
                var pending = await _sharedAccountService.GetPendingInvitationsAsync(userId);
                return Ok(pending);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new shared account with an invite code
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SharedAccountDto>> Create([FromBody] CreateSharedAccountDto dto)
        {
            try
            {
                var userId = GetUserId();
                var sharedAccount = await _sharedAccountService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetUserSharedAccounts), sharedAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Join a shared account using an invite code
        /// </summary>
        [HttpPost("join/{inviteCode}")]
        public async Task<ActionResult<SharedAccountDto>> JoinByCode(string inviteCode)
        {
            try
            {
                var userId = GetUserId();
                var sharedAccount = await _sharedAccountService.JoinByCodeAsync(inviteCode, userId);
                return Ok(sharedAccount);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
        /// Delete a shared account
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                await _sharedAccountService.DeleteAsync(id, userId);
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
    }
}
