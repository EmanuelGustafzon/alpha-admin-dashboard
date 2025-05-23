﻿using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Enums;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Business.Services;

public class ProjectService(IProjectRepository projectRepository, IMemberService memberService, IClientService clientService, IUploadFile uploadFile) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IMemberService _memberService = memberService;
    private readonly IClientService _clientService = clientService;
    private readonly IUploadFile _uploadFile = uploadFile;
    public async Task<ServiceResult<Project>> CreateProjectAsync(ProjectForm projectForm)
    {
        try
        {
            ProjectEntity projectEntity = projectForm.MapTo<ProjectEntity>();
            foreach (var id in projectForm.MemberIds)
            {
                var memberResult = await _memberService.GetMemberByIdAsync(id);
                if (memberResult.Data is not null)
                {
                    var memberProject = new MemberProjectEntity { ProjectId = projectEntity.Id, MemberId = id };
                    projectEntity.MemberProjects.Add(memberProject);
                }
            }
            var clientResult = await _clientService.CreateClientAsync(projectForm.ClientName);
            if (clientResult.Success && clientResult.Data is not null)
            {
                projectEntity.ClientId = clientResult.Data.Id;
            }

            projectEntity.Owner = projectForm.CurrentUserId;

            if (projectForm.Image != null && projectForm.Image.Length != 0)
            {
                string imageUrl = await _uploadFile.UploadFileLocally(projectForm.Image);
                projectEntity.ImageUrl = imageUrl;
            }
            var result = await _projectRepository.CreateAsync(projectEntity);
            if(!result.Succeeded || result.Result is null) return ServiceResult<Project>.Error(result.ErrorMessage ?? "failed to create project");

            return ServiceResult<Project>.Created(result.Result.MapTo<Project>());
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

            var startedProjects = result.Result.Where(project => project.StartDate <= DateTime.Now && project.Status != ProjectStatuses.Completed);
            foreach (var startedProject in startedProjects)
            {
                await UpdateStatusAsync(startedProject.Id, "Started");
            }
            
            var projects = result.Result.Select(x => {
                var project = x.MapTo<Project>();

                if (project.Status == ProjectStatuses.Completed) project.CalculatedTimeDiff = "Completed";
                else project.CalculatedTimeDiff = DateCalculator.GetTimeDiffFromToday(project.StartDate, project.EndDate);

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

    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsAsync(string query)
    {
        try
        {
            var result = await _projectRepository.GetAllAsync( filterBy: x => x.ProjectName.Contains(query) , joins: join => join.Client);

            if (result.Result is not IEnumerable<ProjectEntity>)
                return ServiceResult<IEnumerable<Project>>.Error("Failed to get projects");

            var projects = result.Result.Select(x => {
                var project = x.MapTo<Project>();
                List<Member> members = x.MemberProjects.Select(x => x.Member.MapTo<Member>()).ToList();
                project.Members = members;
                return project;
            }).ToList();

            return ServiceResult<IEnumerable<Project>>.Ok(projects);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<IEnumerable<Project>>.Error("Failed to get projects");
        }
    }
    public async Task<ServiceResult<Project>> GetProjectAsync(string id)
    {
        try
        {
            var result = await _projectRepository.GetAsync(findBy: x =>  x.Id == id, joins: join => join.Client);
            if (result.StatusCode == 404) return ServiceResult<Project>.NotFound($"project with id { id } not found");
            if(!result.Succeeded || result.Result is null)
            {
                return ServiceResult<Project>.Error(result.ErrorMessage ?? "Failed to get project");
                
            }
                var project = result.Result.MapTo<Project>();
                List<Member> members = result.Result.MemberProjects.Select(x => x.Member.MapTo<Member>()).ToList();
                project.Members = members;

            return ServiceResult<Project>.Ok(project);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<Project>.Error("Failed to get project");
        }
    }

    public async Task<ServiceResult<Project>> UpdateProjectAsync(ProjectForm form, string id)
    {
        try
        {
            
            var findProjectResult = await _projectRepository.GetAsync(findBy: x => x.Id == id, joins: join => join.Client);
            if (findProjectResult.StatusCode == 404) return ServiceResult<Project>.NotFound($"project with id {id} not found");
            if(!findProjectResult.Succeeded || findProjectResult.Result is null) return ServiceResult<Project>.Error($"Could not get project :: {findProjectResult.ErrorMessage}");

            var memberIds = findProjectResult.Result.MemberProjects.Select(x => x.MemberId).ToList();
            var permission = await UserHasPermission(form.CurrentUserId, findProjectResult.Result.Owner, memberIds);
            if(permission == false) return ServiceResult<Project>.Unauthorized($"You are not authorized to edit this project");

            var project = findProjectResult.Result!;

            project.ProjectName = form.ProjectName;
            project.Description = form.Description;
            project.StartDate = form.StartDate;
            project.EndDate = form.EndDate;
            project.Budget = form.Budget;

            var membersToRemove = project.MemberProjects.Where(mp => form.MemberIds.Contains(mp.MemberId) == false).ToList();
            foreach (var item in membersToRemove)
            {
                project.MemberProjects.Remove(item);
            }
            foreach (var memberId in form.MemberIds)
            {
                var existingMember = project.MemberProjects.FirstOrDefault(x => x.MemberId == memberId);
                if (existingMember is not null) continue;

                var memberResult = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResult.Data is not null)
                {
                    var memberProject = new MemberProjectEntity { ProjectId = id, MemberId = memberId };
                    project.MemberProjects.Add(memberProject);
                }
            }

            var clientResult = await _clientService.CreateClientAsync(form.ClientName);
            if (clientResult.Success && clientResult.Data is not null)
            {
                project.ClientId = clientResult.Data.Id;
            }

            if (form.Image != null && form.Image.Length != 0)
            {
                string imageUrl = await _uploadFile.UploadFileLocally(form.Image);
                project.ImageUrl = imageUrl;
            }

            var result = await _projectRepository.UpdateAsync(project);
            if(!result.Succeeded || result.Result is null) return ServiceResult<Project>.Error($"Failed to get project :: {result.ErrorMessage}");

            return ServiceResult<Project>.Ok(result.Result.MapTo<Project>());
        }
        catch (Exception ex) 
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<Project>.Error("Failed to get project");
        }
    }

    public async Task<ServiceResult<Project>> UpdateProjectMembersAsync(ProjectMembersForm form, string projectId)
    {
        try
        {
            var findProjectResult = await _projectRepository.GetAsync(findBy: x => x.Id == projectId);
            if (findProjectResult.StatusCode == 404) return ServiceResult<Project>.NotFound($"project with id {projectId} not found");
            if (!findProjectResult.Succeeded && findProjectResult.Result is null) return ServiceResult<Project>.Error($"Could not get project :: {findProjectResult.ErrorMessage}");

            var project = findProjectResult.Result!;

            var memberIds = project.MemberProjects.Select(x => x.MemberId).ToList();
            var permission = await UserHasPermission(form.CurrentUserId, project.Owner, memberIds);
            if (permission == false) return ServiceResult<Project>.Unauthorized($"You do not have persmission to update this project");

            var membersToRemove = project.MemberProjects.Where(mp => form.MemberIds.Contains(mp.MemberId) == false).ToList();
            foreach (var item in membersToRemove)
            {
                project.MemberProjects.Remove(item);
            }
            foreach (var memberId in form.MemberIds)
            {
                var existingMember = project.MemberProjects.FirstOrDefault(x => x.MemberId == memberId);
                if (existingMember is not null) continue;

                var memberResult = await _memberService.GetMemberByIdAsync(memberId);
                if (memberResult.Data is not null)
                {
                    var memberProject = new MemberProjectEntity { ProjectId = projectId, MemberId = memberId };
                    project.MemberProjects.Add(memberProject);
                }
            }

            var result = await _projectRepository.UpdateAsync(project);
            if(result.Result is null) return ServiceResult<Project>.Error($"Failed to add new memebrs :: {result.ErrorMessage}");
        
            return ServiceResult<Project>.Ok(result.Result.MapTo<Project>());
        }
        catch (Exception ex) 
        {
            Debug.Write(ex.Message);
            return ServiceResult<Project>.Error("Failed to add new memebrs");
        }
    }

    public async Task<ServiceResult<Project>> UpdateStatusAsync(string id, string memberId, string status)
    {
        try
        {
            var result = await _projectRepository.GetAsync(x => x.Id == id);
            if (result.Result is null)
                return ServiceResult<Project>.Error("Failed to fetch the project to update status");

            var project = result.Result;

            var memberIds = project.MemberProjects.Select(x => x.MemberId).ToList();
            var permission = await UserHasPermission(memberId, project.Owner, memberIds);
            if (permission == false) return ServiceResult<Project>.Unauthorized($"You do not have persmission to update this project");

            if (!Enum.TryParse(status, true, out ProjectStatuses convertedStatus))
                return ServiceResult<Project>.Error("The status was not of the right enum type");

            project.Status = convertedStatus;

            var updateResult = await _projectRepository.UpdateAsync(project);
            if (updateResult.Result is null)
                return ServiceResult<Project>.Error("Failed to update project status");

            return ServiceResult<Project>.Ok(project.MapTo<Project>());
        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
            return ServiceResult<Project>.Error("Failed to update project status");
        }
    }

    private async Task<ServiceResult<Project>> UpdateStatusAsync(string id, string status)
    {
        try
        {
            var result = await _projectRepository.GetAsync(x => x.Id == id);
            if (result.Result is null)
                return ServiceResult<Project>.Error("Failed to fetch the project to update status");

            var project = result.Result;

            if (!Enum.TryParse(status, true, out ProjectStatuses convertedStatus))
                return ServiceResult<Project>.Error("The status was not of the right enum type");

            project.Status = convertedStatus;

            var updateResult = await _projectRepository.UpdateAsync(project);
            if (updateResult.Result is null)
                return ServiceResult<Project>.Error("Failed to update project status");

            return ServiceResult<Project>.Ok(project.MapTo<Project>());
        }
        catch (Exception ex)
        {
            Debug.Write(ex.Message);
            return ServiceResult<Project>.Error("Failed to update project status");
        }
    }


    public async Task<ServiceResult<bool>> DeleteProjectAsync(string projectId, string memberId)
    {
        try
        {
            var findProjectResult = await _projectRepository.GetAsync(x =>  x.Id == projectId);
            if(findProjectResult.Result is null) return ServiceResult<bool>.NotFound($"Project not found :: {findProjectResult.ErrorMessage}");

            var memberIds = findProjectResult.Result.MemberProjects.Select(x => x.MemberId).ToList();
            var permission = await UserHasPermission(memberId, findProjectResult.Result.Owner, memberIds);
            if (permission == false) return ServiceResult<bool>.Unauthorized($"You do not have persmission to delete this project");

            var result = await _projectRepository.DeleteAsync(x => x.Id == projectId);
            if(!result.Succeeded) return ServiceResult<bool>.Error($"Failed to delete project :: {result.ErrorMessage}");

            return ServiceResult<bool>.NoContent();
        } catch (Exception ex) 
        { 
            Debug.Write(ex.Message);
            return ServiceResult<bool>.Error("Failed to delete project");
        }
    }

    private async Task<bool> UserHasPermission(string loggedInUserId, string projectOwnerId, List<string> memberIds)
    {
        var roles = await _memberService.GetMemeberRoles(loggedInUserId);
        if (roles.Data != null && roles.Data.Contains("Admin")) return true;
        if(projectOwnerId == loggedInUserId) return true;
        foreach(var memberId in memberIds)
        {
            if (memberId == loggedInUserId) return true;
        }
        return false;
    }
}
