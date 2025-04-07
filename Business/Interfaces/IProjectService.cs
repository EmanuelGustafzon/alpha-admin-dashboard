using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IProjectService
{
    public Task<ServiceResult<Project>> CreateProjectAsync(ProjectForm projectForm, string memberId);

    public Task<ServiceResult<IEnumerable<Project>>> GetProjectsAsync();
}
