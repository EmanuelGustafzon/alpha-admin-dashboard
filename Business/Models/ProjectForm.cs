using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Business.Models
{
    public class ProjectForm
    {
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; } = "/images/default-project-image.png";

        [Required]
        [Display(Name = "project Name")]
        public string ProjectName { get; set; } = null!;

        [Required]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; } = null!;

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [Required]
        public string CurrentUserId { get; set; } = null!;

        [Display(Name = "Members")]
        public List<string> MemberIds { get; set; } = [];

        [Display(Name = "Budget")]
        public decimal Budget { get; set; }
    }
}
