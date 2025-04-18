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

    public Task<ServiceResult<Project>> UpdateProjectMembersAsync(ProjectMembersForm form, string projectId);

    public Task<ServiceResult<Project>> UpdateStatusAsync(string id, string status);

    public Task<ServiceResult<bool>> DeleteProjectAsync(string projectId);
}
