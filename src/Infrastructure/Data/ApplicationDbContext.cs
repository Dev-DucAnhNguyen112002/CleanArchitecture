using System;
using System.Collections.Generic;
using CleanArchitectureTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTest.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Normalizedname, "ix_roles_normalizedname");

            entity.HasIndex(e => e.Normalizedname, "roles_normalizedname_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Created).HasColumnName("created").HasColumnType("timestamp with time zone");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.LastModified).HasColumnName("last_modified").HasColumnType("timestamp with time zone");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(256)
                .HasColumnName("last_modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Normalizedname)
                .HasMaxLength(256)
                .HasColumnName("normalizedname");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Normalizedusername, "ix_users_normalizedusername");

            entity.HasIndex(e => e.Normalizedemail, "users_normalizedemail_key").IsUnique();

            entity.HasIndex(e => e.Normalizedusername, "users_normalizedusername_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Accessfailedcount)
                .HasDefaultValue(0)
                .HasColumnName("accessfailedcount");
            entity.Property(e => e.Created).HasColumnName("created").HasColumnType("timestamp with time zone");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.Emailconfirmed)
                .HasDefaultValue(false)
                .HasColumnName("emailconfirmed");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.LastModified).HasColumnName("last_modified").HasColumnType("timestamp with time zone");
            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(256)
                .HasColumnName("last_modified_by");
            entity.Property(e => e.Lockoutenabled)
                .HasDefaultValue(false)
                .HasColumnName("lockoutenabled");
            entity.Property(e => e.Lockoutend).HasColumnName("lockoutend").HasColumnType("timestamp with time zone");
            entity.Property(e => e.Normalizedemail)
                .HasMaxLength(256)
                .HasColumnName("normalizedemail");
            entity.Property(e => e.Normalizedusername)
                .HasMaxLength(256)
                .HasColumnName("normalizedusername");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(50)
                .HasColumnName("phonenumber");
            entity.Property(e => e.Phonenumberconfirmed)
                .HasDefaultValue(false)
                .HasColumnName("phonenumberconfirmed");
            entity.Property(e => e.Twofactorenabled)
                .HasDefaultValue(false)
                .HasColumnName("twofactorenabled");
            entity.Property(e => e.Username)
                .HasMaxLength(256)
                .HasColumnName("username");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("Roleid")
                        .HasConstraintName("fk_userroles_roles_roleid"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Userid")
                        .HasConstraintName("fk_userroles_users_userid"),
                    j =>
                    {
                        j.HasKey("Userid", "Roleid").HasName("pk_userroles");
                        j.ToTable("user_roles");
                        j.IndexerProperty<Guid>("Userid").HasColumnName("userid");
                        j.IndexerProperty<Guid>("Roleid").HasColumnName("roleid");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
