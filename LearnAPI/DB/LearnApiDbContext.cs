using System;
using System.Collections.Generic;
using LearnAPI.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.DB;

public partial class LearnAPIDbContext : DbContext
{
    public LearnAPIDbContext()
    {
    }

    public LearnAPIDbContext(DbContextOptions<LearnAPIDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    public virtual DbSet<TblToken> TblTokens { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_toke__3213E83F6ED19C91");

            entity.HasOne(d => d.User).WithMany(p => p.TblTokens).HasConstraintName("FK__tbl_token__useri__32E0915F");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__tbl_user__A25C5AA6C95C31EF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
