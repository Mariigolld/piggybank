using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    public class GroupsController : BaseApiController
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Dohvata sve grupe korisnika
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetUserGroups()
        {
            try
            {
                var userId = GetUserId();
                var groups = await _groupService.GetUserGroupsAsync(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dohvata detalje grupe
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDetailsDto>> GetGroupDetails(int id)
        {
            try
            {
                var userId = GetUserId();
                var group = await _groupService.GetGroupDetailsAsync(id, userId);
                return Ok(group);
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
        /// Kreira novu grupu
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GroupDto>> Create([FromBody] CreateGroupDto dto)
        {
            try
            {
                var userId = GetUserId();
                var group = await _groupService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetGroupDetails), new { id = group.Id }, group);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Briše grupu
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                await _groupService.DeleteAsync(id, userId);
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
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Pridruživanje grupi putem pozivnog koda
        /// </summary>
        [HttpPost("join/{inviteCode}")]
        public async Task<ActionResult<GroupDto>> JoinByCode(string inviteCode)
        {
            try
            {
                var userId = GetUserId();
                var group = await _groupService.JoinByCodeAsync(inviteCode, userId);
                return Ok(group);
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
        /// Kalkuliše settlements (ko kome duguje)
        /// </summary>
        [HttpGet("{id}/settlements")]
        public async Task<ActionResult<GroupSettlementsDto>> CalculateSettlements(int id)
        {
            try
            {
                var userId = GetUserId();
                var settlements = await _groupService.CalculateSettlementsAsync(id, userId);
                return Ok(settlements);
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
        /// Beleži izravnanje (plaćanje) članu grupe
        /// </summary>
        [HttpPost("{id}/settle")]
        public async Task<ActionResult<GroupSettlementDto>> RecordSettlement(int id, [FromBody] CreateGroupSettlementDto dto)
        {
            try
            {
                if (dto.GroupId != id)
                {
                    dto.GroupId = id;
                }

                var userId = GetUserId();
                var settlement = await _groupService.RecordSettlementAsync(dto, userId);
                return CreatedAtAction(nameof(GetSettlementHistory), new { id = id }, settlement);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dohvata istoriju izravnanja grupe
        /// </summary>
        [HttpGet("{id}/settlement-history")]
        public async Task<ActionResult<IEnumerable<GroupSettlementDto>>> GetSettlementHistory(int id)
        {
            try
            {
                var userId = GetUserId();
                var settlements = await _groupService.GetGroupSettlementsAsync(id, userId);
                return Ok(settlements);
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
