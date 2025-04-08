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

public class ProjectService(IProjectRepository projectRepository, IMemberService memberService, IClientService clientService, IUploadFile uploadFile) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IMemberService _memberService = memberService;
    private readonly IClientService _clientService = clientService;
    private readonly IUploadFile _uploadFile = uploadFile;
    public async Task<ServiceResult<Project>> CreateProjectAsync(ProjectForm projectForm, string memberId)
    {
        try
        {
            ProjectEntity projectEntity = projectForm.MapTo<ProjectEntity>();
            foreach (var id in projectForm.MemberIds)
            {
                var res = await _memberService.GetMemberByIdAsync(id);
                if (res.Data is not null)
                {
                    var memberProject = new MemberProjectEntity { ProjectId = projectEntity.Id, MemberId = id };
                    projectEntity.MemberProjects.Add(memberProject);
                }
            }
            var clientResult = await _clientService.CreateClientAsync(projectForm.ClientName);
            if (clientResult.Success)
            {
                projectEntity.ClientId = clientResult.Data.Id;
            }

            projectEntity.Owner = memberId;

            if (projectForm.Image != null && projectForm.Image.Length != 0)
            {
                string imageUrl = await _uploadFile.UploadFileLocally(projectForm.Image);
                projectEntity.ImageUrl = imageUrl;
            }
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
            var result = await _projectRepository.GetAllAsync(joins: join => join.Client);
            
            if (result.Result is not IEnumerable<ProjectEntity>)
                return ServiceResult<IEnumerable<Project>>.Error("Failed to get projects");

            var projects = result.Result.Select(x => {
                var project = x.MapTo<Project>();
                List<Member> members = x.MemberProjects.Select(x => x.Member.MapTo<Member>()).ToList();
                project.Members = members;
                return project;
                }).ToList();
                
            return ServiceResult<IEnumerable<Project>>.Ok(projects);
        } catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<IEnumerable<Project>>.Error("Failed to get projects");
        }
    }
}
