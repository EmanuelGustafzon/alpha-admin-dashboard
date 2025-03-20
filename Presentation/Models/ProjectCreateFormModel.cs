using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class ProjectCreateFormModel
{
    [Required(ErrorMessage = "Project Name is Required")]
    [Display(Name = "Project Name", Prompt = "Enter Project Name")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "Client Name is Required")]
    [Display(Name = "Client Name", Prompt = "Enter Client Name")]
    public string ClientName { get; set; } = null!;

    [Display(Name = "Description", Prompt = "Enter a Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start Date is Required")]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "End Date is Required")]
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [Display(Name = "Budget")]
    public int Budget { get; set; } 
}
