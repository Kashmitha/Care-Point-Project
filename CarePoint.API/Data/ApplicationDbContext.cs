using Microsoft.EntityFrameworkCore;
using CarePoint.API.Models;

namespace CarePoint.API.Data 
{
    // Main database context for CarePoint app
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        // DbSets represent tables in database
        public DbSet Users { get; set; }
        public DbSet Specialties { get; set; }
        public DbSet Doctors { get; set; }
        public DbSet DoctorAvailabilities { get; set; }
        public DbSet Appointments { get; set; }
        public DbSet Prescriptions { get; set; }
        public DbSet PrescriptionsMedications { get; set; }
        public DbSet MedicalRecords { get; set; }
        public DbSet Notofications { get; set; }
        public DbSet Reviews { get; set; }

        // Configure entity relationships and constraints
        // This is where configure
        // - Relationships ( 1.1, 1.m, n.m)
        // - Unique constraints
        // - Default values
        // - Indexes for query performance

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity(entity => 
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("Patient");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Doctor configurations
            modelBuilder.Entity(entity => 
            {
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.HasOne(d => d.User)
                    .WithOne(u => u.Doctor)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Specialty configurations
            modelBuilder.Entity(entity => 
            {
                entity.HasIndex(e => e.SpecialtyName).IsUnique();
            });

            // Appointment configurations
            modelBuilder.Entity(entity => 
            {
                entity.HasOne(a => a.Patient)
                    .WithMany(u => u.AppointmentAsPatient)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.Property(a => a.Status).HasDefaultvalue("Scheduled");
            });

            // Prescription configurations
            modelBuilder.Entity(entity => 
            {
                entity.HasOne(p => p.Appointment)
                    .WithOne(a => a.Prescription)
                    .HasForeignKey(p => p.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial data (good for testing) 
            SeedData(modelBuilder);
        }

        // Seed initial data for testing
        // - Initial admin accounts
        // - Reference data (specialties)
        // - Test data for development

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed specialties
            modelBuilder.Entity().HasData(
                new Specialty { SpecialtyId = 1, SpecialtyName = "Cardiology", Description = "Heart and cardiovascular system", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 2, SpecialtyName = "Neurology", Description = "Brain and nervous system", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 3, SpecialtyName = "Pediatrics", Description = "Children's health", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 4, SpecialtyName = "Orthopedics", Description = "Bones, joints, and muscles", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 5, SpecialtyName = "Dermatology", Description = "Skin, hair, and nails", CreatedAt = DateTime.UtcNow }
            );

            // Seed Admin User (Password: Admin@123)
            string adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            modelBuilder.Entity.Entity().HasData(
                new User
                {
                    UserId = 1,
                    Email = "admin@carepoint.com",
                    PasswordHash = adminPasswordHash,
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}