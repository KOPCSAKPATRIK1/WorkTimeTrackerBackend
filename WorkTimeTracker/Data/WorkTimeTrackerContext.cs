using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.Core.Models.Domain;

namespace WorkTimeTracker.Data;

public partial class WorkTimeTrackerContext : DbContext
{
    public WorkTimeTrackerContext()
    {
    }

    public WorkTimeTrackerContext(DbContextOptions<WorkTimeTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<WorkTask> Tasks { get; set; }

    public virtual DbSet<TimeEntry> TimeEntries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WorkTimeTracker;Trusted_Connection=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC072CBBE5E5");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Projects__Create__2A4B4B5E");

            entity.HasOne(d => d.ParentProject).WithMany(p => p.InverseParentProject)
                .HasForeignKey(d => d.ParentProjectId)
                .HasConstraintName("FK__Projects__Parent__29572725");
        });

        modelBuilder.Entity<WorkTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tasks__3214EC07C7D94561");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Tasks__ProjectId__2E1BDC42");
        });

        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TimeEntr__3214EC072F7E7CDE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Hours).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Project).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TimeEntri__Proje__32E0915F");

            entity.HasOne(d => d.Task).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TimeEntri__TaskI__33D4B598");

            entity.HasOne(d => d.User).WithMany(p => p.TimeEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TimeEntri__UserI__31EC6D26");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC070E4F1DD3");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345E52F199").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
