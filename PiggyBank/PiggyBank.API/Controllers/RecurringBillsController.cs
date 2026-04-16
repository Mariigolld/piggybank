using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    [Route("api/SharedAccounts/{sharedAccountId}/RecurringBills")]
    public class RecurringBillsController : BaseApiController
    {
        private readonly IRecurringBillService _recurringBillService;

        public RecurringBillsController(IRecurringBillService recurringBillService)
        {
            _recurringBillService = recurringBillService;
        }

        /// <summary>Get all recurring bills for a shared account</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurringBillDto>>> GetAll(int sharedAccountId)
        {
            try
            {
                var userId = GetUserId();
                var bills = await _recurringBillService.GetBySharedAccountAsync(sharedAccountId, userId);
                return Ok(bills);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Get a specific recurring bill</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RecurringBillDto>> GetById(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                var bill = await _recurringBillService.GetByIdAsync(id, userId);
                return Ok(bill);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Create a new recurring bill</summary>
        [HttpPost]
        public async Task<ActionResult<RecurringBillDto>> Create(int sharedAccountId, [FromBody] CreateRecurringBillDto dto)
        {
            try
            {
                var userId = GetUserId();
                var bill = await _recurringBillService.CreateAsync(sharedAccountId, dto, userId);
                return CreatedAtAction(nameof(GetById), new { sharedAccountId, id = bill.Id }, bill);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>Mark a payment as paid</summary>
        [HttpPost("{id}/payments/{year}/{month}/paid")]
        public async Task<ActionResult<RecurringBillPaymentDto>> MarkPaid(int sharedAccountId, int id, int year, int month, [FromBody] MarkBillPaidDto dto)
        {
            try
            {
                var userId = GetUserId();
                var payment = await _recurringBillService.MarkPaymentPaidAsync(id, month, year, userId, dto);
                return Ok(payment);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Mark a payment as unpaid</summary>
        [HttpPost("{id}/payments/{year}/{month}/unpaid")]
        public async Task<ActionResult<RecurringBillPaymentDto>> MarkUnpaid(int sharedAccountId, int id, int year, int month)
        {
            try
            {
                var userId = GetUserId();
                var payment = await _recurringBillService.MarkPaymentUnpaidAsync(id, month, year, userId);
                return Ok(payment);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Reactivate a paused recurring bill</summary>
        [HttpPatch("{id}/reactivate")]
        public async Task<ActionResult> Reactivate(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                await _recurringBillService.ReactivateAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Deactivate a recurring bill</summary>
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                await _recurringBillService.DeactivateAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        /// <summary>Delete a recurring bill</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int sharedAccountId, int id)
        {
            try
            {
                var userId = GetUserId();
                await _recurringBillService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }
    }
}
