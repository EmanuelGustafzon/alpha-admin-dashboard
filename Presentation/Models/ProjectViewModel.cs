namespace Presentation.Models;

public class ProjectViewModel
{
    public IEnumerable<Project> Projects { get; set; } = [];
    public ProjectCreateFormModel CreateProjectForm { get; set; } = new ProjectCreateFormModel();

}
