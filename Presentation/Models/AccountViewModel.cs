using Business.Models;
using Domain.Models;

namespace Presentation.Models;

public class AccountViewModel
{
    public MemberForm EditAccountForm { get; set; } = new();

    public Member CurrentUserAccount { get; set; } = new();

    public ChangePasswordForm ChangePasswordForm { get; set; } = new();

    public bool CurrentUserHasExternalprovider { get; set; } = true;
}
