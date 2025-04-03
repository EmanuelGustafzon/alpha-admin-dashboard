using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ProjectForm
    {

        public string? Description { get; set; }
        public List<string> MemberIds { get; set; } = [];
    }
}
