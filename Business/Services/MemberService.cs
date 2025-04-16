using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace Business.Services;
public class MemberService(
    UserManager<MemberEntity> userManager, 
    IMemberRepository memberRepository,
    IUploadFile uploadFile
    ) : IMemberService
{
    private readonly UserManager<MemberEntity> _userManager = userManager;
    private readonly IMemberRepository _memberRepository = memberRepository;
    private readonly IUploadFile _uploadFile = uploadFile;
    public async Task<ServiceResult<MemberEntity>> CreateMemberAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            var existingMember = await _userManager.FindByEmailAsync(email);
            if (existingMember != null)
                return ServiceResult<MemberEntity>.Conflict("Email is already in use");

            MemberEntity member = MemberFactory.CreateEntity(firstName, lastName, email);
            IdentityResult result = await _userManager.CreateAsync(member, password);

            if (!result.Succeeded)
                return ServiceResult<MemberEntity>.Error("Failed to Create Accont");

            var thisIsTheFirstUser = _userManager.Users.Count() == 1;
            if (thisIsTheFirstUser)
                await _userManager.AddToRoleAsync(member, "Admin");

            return ServiceResult<MemberEntity>.Created(member);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<MemberEntity>.Error("Failed to Create User");
        }
    }
    public async Task<ServiceResult<MemberEntity>> CreateMemberAsync(string firstName, string lastName, string email)
    {
        try
        {
            var existingMember = await _userManager.FindByEmailAsync(email);
            if (existingMember != null)
                return ServiceResult<MemberEntity>.Conflict("Email is already in use");

            MemberEntity member = MemberFactory.CreateEntity(firstName, lastName, email);
            IdentityResult result = await _userManager.CreateAsync(member);

            if (!result.Succeeded)
                return ServiceResult<MemberEntity>.Error("Failed to Create Accont");

            var thisIsTheFirstUser = _userManager.Users.Count() == 1;
            if (thisIsTheFirstUser)
                await _userManager.AddToRoleAsync(member, "Admin");

            return ServiceResult<MemberEntity>.Created(member);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<MemberEntity>.Error("Failed to Create User");
        }
    }

    public async Task<ServiceResult<string>> CreateMemberAsync(MemberWithRoleForm form)
    {
        try
        {

            var memberEntity = form.MapTo<MemberEntity>();
            memberEntity.UserName = form.Email;

            if (memberEntity.Address == null)
                memberEntity.Address = new MemberAddressEntity { MemberId = memberEntity.Id };
            memberEntity.Address.City = form.AddressForm.City;
            memberEntity.Address.PostCode = form.AddressForm.PostCode;
            memberEntity.Address.Street = form.AddressForm.Street;

            if (form.Image != null && form.Image.Length != 0)
            {
                string imageUrl = await _uploadFile.UploadFileLocally(form.Image);
                memberEntity.ImageUrl = imageUrl;
            }

            string generatedPassword = $"{form.FirstName.ToUpper()}-{Guid.NewGuid().ToString()[..4]}";

            var result = await _userManager.CreateAsync(memberEntity, generatedPassword);
            if(!result.Succeeded) return ServiceResult<string>.Error($"Failed to create member {result.Errors.ToString()}");

            var roleResult = await _userManager.AddToRoleAsync(memberEntity, form.Role);

            return ServiceResult<string>.Created(generatedPassword);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<string>.Error("Failed to create member");
        }
    }

    public async Task<ServiceResult<IEnumerable<Member>>> GetAllMembersAsync()
    {
        var result = await _memberRepository.GetAllAsync();
        if(!result.Succeeded)
        {
            return ServiceResult<IEnumerable<Member>>.Error("failed to fetch members");
        }
        if(result.Result is not null)
        {
            var members = result.Result.Select(m => m.MapTo<Member>()).ToList();
            return ServiceResult<IEnumerable<Member>>.Ok(members);
        }
        return ServiceResult<IEnumerable<Member>>.Ok([]);
    }

    public async Task<ServiceResult<Member>> GetMemberByIdAsync(string id)
    {
        try
        {
            var result = await _memberRepository.GetAsync(
                entity => entity.Id == id,
                join => join.Address
                );

            if (result.Result == null)
                return ServiceResult<Member>.NotFound("Could not find member");

            var member = result.Result.MapTo<Member>();
                
            return ServiceResult<Member>.Ok(member);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<Member>.Error("Failed to fetch member");
        }
    }
    public async Task<ServiceResult<Member>> UpdateMemberAsync(MemberForm form, string id)
    {
        try
        {
            var repositoryResult = await _memberRepository.GetAsync(
                entity => entity.Id == id,
                join => join.Address
                );

            if (repositoryResult.Result == null)
                return ServiceResult<Member>.NotFound("Could not find member");

            var memberEntity = repositoryResult.Result;

            memberEntity.ImageUrl = form.ImageUrl;
            if (form.Image != null && form.Image.Length != 0)
            {
                string imageUrl = await _uploadFile.UploadFileLocally(form.Image);
                memberEntity.ImageUrl = imageUrl;
            }
            memberEntity.FirstName = form.FirstName;
            memberEntity.LastName = form.LastName;
            memberEntity.JobTitle = form.JobTitle;
            memberEntity.PhoneNumber = form.PhoneNumber;
            memberEntity.BirthDate = form.BirthDate;

            var useExternalprovider = await MemberUseExternalProvider(memberEntity);
            if (useExternalprovider == false) 
            {
                memberEntity.Email = form.Email;
                memberEntity.UserName = form.Email;
            }

            if (memberEntity.Address == null)
                memberEntity.Address = new MemberAddressEntity { MemberId = memberEntity.Id };

            memberEntity.Address.City = form.AddressForm.City;
            memberEntity.Address.PostCode = form.AddressForm.PostCode;
            memberEntity.Address.Street = form.AddressForm.Street;
            
            var identityResult = await _userManager.UpdateAsync(memberEntity);
            if(!identityResult.Succeeded)
                return ServiceResult<Member>.Error("Failed to update member");

            MemberAddress address = memberEntity.Address.MapTo<MemberAddress>();
            var member = memberEntity.MapTo<Member>();
            member.Address = address;

            return ServiceResult<Member>.Ok(member);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<Member>.Error("Failed to update member");
        }
    }
    public async Task<bool> MemberUseExternalProvider(MemberEntity member)
    {
        var logins = await _userManager.GetLoginsAsync(member);
        if (logins.Count > 0 && logins[0].LoginProvider != null)
        {
            return true;
        }
        return false;
    }
    public async Task<bool?> MemberUseExternalProvider(string memberId)
    {
        var member = await _userManager.FindByIdAsync(memberId);
        if(member == null) return null;
        var logins = await _userManager.GetLoginsAsync(member);
        if(logins.Count > 0 && logins[0].LoginProvider != null)
        {
            return true;
        }
        return false;
    }
}
