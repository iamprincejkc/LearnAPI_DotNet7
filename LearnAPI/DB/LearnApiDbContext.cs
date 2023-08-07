using System;
using System.Collections.Generic;
using LearnAPI.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.DB;

public partial class LearnApiDbContext : DbContext
{
    public LearnApiDbContext()
    {
    }

    public LearnApiDbContext(DbContextOptions<LearnApiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__tbl_user__A25C5AA6C95C31EF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
