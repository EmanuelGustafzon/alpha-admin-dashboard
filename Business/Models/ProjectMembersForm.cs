using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ProjectMembersForm
{
    [Display(Name = "Members")]
    public List<string> MemberIds { get; set; } = [];

    [Required]
    public string CurrentUserId { get; set; } = null!;
}
