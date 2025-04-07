using Business.Models;
using Domain.Models;

namespace Presentation.Models
{
    public class ProjectViewModel
    {
        public ProjectForm ProjectForm { get; set; } = new();
        public IEnumerable<Member> Members { get; set; } = [];
        public IEnumerable<Project> Projects { get; set; } = [];
    }
}
