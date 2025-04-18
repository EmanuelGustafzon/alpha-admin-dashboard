using Business.Models;
using Data.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata;


namespace Business.Interfaces;

public interface IMemberService
{
    public Task<ServiceResult<MemberEntity>> CreateMemberAsync(string firstName, string lastName, string email, string password);

    public Task<ServiceResult<MemberEntity>> CreateMemberAsync(string firstName, string lastName, string email);

    public Task<ServiceResult<string>> CreateMemberAsync(MemberWithRoleForm form);

    public Task<ServiceResult<IEnumerable<Member>>> GetAllMembersAsync();

    public Task<ServiceResult<IEnumerable<Member>>> GetAllMembersAsync(string query);

    public Task<ServiceResult<Member>> GetMemberByIdAsync(string id);

    public Task<ServiceResult<Member>> UpdateMemberAsync(MemberForm form, string id);

    public Task<ServiceResult<Member>> UpdateMemberAsync(MemberWithRoleForm form, string id);

    public Task<ServiceResult<Member>> DeleteMemberAsync(string id);

    public Task<ServiceResult<bool>> ChangePasswordAsync(string id, ChangePasswordForm form);

    public Task<bool> MemberUseExternalProvider(MemberEntity member);

    public Task<bool?> MemberUseExternalProvider(string memberId);
}
