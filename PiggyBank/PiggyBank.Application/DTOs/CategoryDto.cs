using System.ComponentModel.DataAnnotations;

namespace PiggyBank.Application.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsShared { get; set; }
        public string Icon { get; set; } = "📁";
        public string Color { get; set; } = "#3B82F6";
    }

    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsShared { get; set; } = false;

        [MaxLength(10)]
        public string Icon { get; set; } = "📁";

        [MaxLength(20)]
        public string Color { get; set; } = "#3B82F6";
    }
}