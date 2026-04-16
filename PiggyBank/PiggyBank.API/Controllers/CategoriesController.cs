using Microsoft.AspNetCore.Mvc;
using PiggyBank.Application.DTOs;
using PiggyBank.Application.Services;

namespace PiggyBank.API.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Dohvata sve kategorije (korisničke i deljene)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetUserCategories()
        {
            try
            {
                var userId = GetUserId();
                var categories = await _categoryService.GetUserCategoriesAsync(userId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Kreira novu kategoriju
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
        {
            try
            {
                var userId = GetUserId();
                var category = await _categoryService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetUserCategories), category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Briše kategoriju
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                await _categoryService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                // Use 403 Forbidden for permission denied, not 401 (which is for auth failures)
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}