using Business.Models;
using Data.Entities;
using Domain.Models;


namespace Business.Interfaces;

public interface IMemberService
{
    public Task<ServiceResult<MemberEntity>> CreateMemberAsync(string firstName, string lastName, string email, string password);

    public Task<ServiceResult<MemberEntity>> CreateMemberAsync(string firstName, string lastName, string email);

    public Task<ServiceResult<IEnumerable<Member>>> GetAllMembersAsync();
    public Task<ServiceResult<Member>> GetMemberByIdAsync(string id);

    public Task<ServiceResult<Member>> UpdateMemberAsync(MemberForm form, string id);

    public Task<bool> MemberUseExternalProvider(MemberEntity member);

    public Task<bool?> MemberUseExternalProvider(string memberId);
}
