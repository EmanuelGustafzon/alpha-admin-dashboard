using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class DeleteProject
{
    [Required]
    [Display(Name = "Project Id")]
    public string Id { get; set; } = null!;
}
