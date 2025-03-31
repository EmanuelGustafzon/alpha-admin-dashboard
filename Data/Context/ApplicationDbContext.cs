﻿using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public class ApplicationDbContext : IdentityDbContext<MemberEntity>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }
    public DbSet<MemberAddressEntity> MemberAddresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MemberEntity>()
            .HasOne(m => m.Address) 
            .WithOne(a => a.Member) 
            .HasForeignKey<MemberAddressEntity>(a => a.MemberId)
            .IsRequired(); 
    }
}
