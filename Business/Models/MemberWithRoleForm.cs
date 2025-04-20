using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class MemberWithRoleForm : MemberForm
{
    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; } = "User";
}
