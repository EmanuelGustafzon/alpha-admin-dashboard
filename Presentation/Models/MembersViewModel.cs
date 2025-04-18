using Business.Models;
using Domain.Models;

namespace Presentation.Models;

public class MembersViewModel
{
    public IEnumerable<Member> Members { get; set; } = [];

    public MemberWithRoleForm MemberWithRoleForm { get; set; } = new();
}
