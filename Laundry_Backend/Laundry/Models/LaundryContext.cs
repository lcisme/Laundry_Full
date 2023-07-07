using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Laundry.Models
{
    public partial class LaundryContext : DbContext
    {
        public LaundryContext()
        {
        }

        public LaundryContext(DbContextOptions<LaundryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<Membership> Memberships { get; set; } = null!;
        public virtual DbSet<Package> Packages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=CUONGBEOS\\SQLEXPRESS03;Initial Catalog=Laundry;Persist Security Info=True;User ID=sa;Password=123456;Database=Laundry;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.Phone, "UQ__Customer__5C7E359EA4A7A06F")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Mail)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoice");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Invoice__Custome__3F466844");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("FK__Invoice__Type__403A8C7D");
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.ToTable("Membership");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Memberships)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Membershi__Custo__3C69FB99");
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.ToTable("Package");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
