using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Business.Models
{
    public class ProjectForm
    {
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string ProjectName { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<string> MemberIds { get; set; } = [];
        public decimal Budget { get; set; }
    }
}
