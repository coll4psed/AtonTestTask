using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AtonTestTask.Entities;

namespace AtonTestTask.Context
{
    public partial class AtonTestTaskContext : DbContext
    {
        public AtonTestTaskContext()
        {
        }

        public AtonTestTaskContext(DbContextOptions<AtonTestTaskContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=AtonTestTask;Uid=postgres;pwd=admin;"); 
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Guid)
                    .HasName("User_pkey");

                entity.ToTable("User");

                entity.HasIndex(e => e.Login, "User_Login_key")
                    .IsUnique();

                entity.Property(e => e.Guid).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
