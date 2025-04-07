﻿using Domain.Enums;

namespace Domain.Models;

public class Project
{
    public string? ImageUrl { get; set; }
    public string ProjectName { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatuses Status { get; set; }
    public List<Member> Members { get; set; } = [];
    public decimal Budget { get; set; }
}
