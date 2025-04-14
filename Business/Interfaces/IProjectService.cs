using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IProjectService
{
    public Task<ServiceResult<Project>> CreateProjectAsync(ProjectForm projectForm, string memberId);

    public Task<ServiceResult<IEnumerable<Project>>> GetProjectsAsync();

    public Task<ServiceResult<IEnumerable<Project>>> GetProjectsAsync(string query);

    public Task<ServiceResult<Project>> GetProjectAsync(string id);

    public Task<ServiceResult<Project>> UpdateProjectAsync(ProjectForm form, string id);

    public Task<ServiceResult<Project>> UpdateProjectMembersAsync(List<string> memberIds, string projectId);

    public Task<ServiceResult<bool>> DeleteProjectAsync(string projectId);
}
