using Business.Interfaces;
using Business.Models;
using Domain.Models;
using Data.Repositories;
using Data.Interfaces;
using System.Diagnostics;
using Domain.Extensions;
using Data.Entities;
using System.Threading.Tasks.Dataflow;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository, IMemberService memberService, IClientService clientService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IMemberService _memberService = memberService;
    private readonly IClientService _clientService = clientService;
    public async Task<ServiceResult<Project>> CreateProjectAsync(ProjectForm projectForm, string memberId)
    {
        try
        {
            ProjectEntity projectEntity = projectForm.MapTo<ProjectEntity>();
            foreach (var id in projectForm.MemberIds)
            {
                var res = await _memberService.GetMemberByIdAsync(id);
                if (res.Success)
                {
                    projectEntity.MemberProjects.Add(new MemberProjectEntity { MemberId = res.Data.Id, ProjectId = projectEntity.Id });
                }
            }
            var clientResult = await _clientService.CreateClientAsync(projectForm.ClientName);
            if (clientResult.Success)
            {
                projectEntity.ClientId = clientResult.Data.Id;
            }
            projectEntity.Owner = memberId;
            var result = await _projectRepository.CreateAsync(projectEntity);

            return ServiceResult<Project>.NoContent();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<Project>.Error("Failed to create project");
        }
    }

    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsAsync()
    { 
        try
        {
            var result = await _projectRepository.GetAllAsync(
                joins: [join => join.MemberProjects, join => join.Client]);

            if (result.Result is not IEnumerable<ProjectEntity>)
                return ServiceResult<IEnumerable<Project>>.Error("Failed to get projects");

            var projects = result.Result.Select(x => {
                var project = x.MapTo<Project>();
                project.Members = x.MemberProjects.Select(x => x.MapTo<Member>()).ToList();
                return project;
                });

            return ServiceResult<IEnumerable<Project>>.Ok(projects);
        } catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<IEnumerable<Project>>.Error("Failed to get projects");
        }
    }
}
