using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ProjectForm
    {
        public List<string> MemberIds { get; set; } = [];
    }
}
