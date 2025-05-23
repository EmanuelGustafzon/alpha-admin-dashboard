﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class MemberEntity : IdentityUser
{
    [Column(TypeName = "varchar(200)")]
    [PersonalData]
    public string FirstName { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    [PersonalData]
    public string LastName { get; set; } = null!;

    [PersonalData]
    [Column(TypeName = "varchar(400)")]
    public string? ImageUrl { get; set; }

    [Column(TypeName = "varchar(200)")]
    [PersonalData]
    public string? JobTitle { get; set; }

    [ProtectedPersonalData]
    public DateTime? BirthDate { get; set; }

    [ProtectedPersonalData]
    public MemberAddressEntity? Address { get; set; }

    public ICollection<MemberProjectEntity>? MemberProjects { get; set; }

    public ICollection<NotificationDissmissEntity> DissmissedNotifications = [];
}
