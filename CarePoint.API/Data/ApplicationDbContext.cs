using Microsoft.EntityFrameworkCore;
using CarePoint.API.Models;

namespace CarePoint.API.Data 
{
    // Main database context for CarePoint application.
    public class ApplicationDbContext : DbContext
    {
        // FIXED: Added generic type parameter
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets represent tables in database
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Specialty> Specialties { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Prescription> Prescriptions { get; set; } = null!;
        public DbSet<PrescriptionMedication> PrescriptionMedications { get; set; } = null!;
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        // Configure entity relationships and constraints.
        // OnModelCreating is where you configure:
        // - Relationships (one-to-one, one-to-many, many-to-many)
        // - Indexes for query performance
        // - Unique constraints
        // - Default values
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity => 
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("Patient");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Doctor configurations
            modelBuilder.Entity<Doctor>(entity => 
            {
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.HasOne(d => d.User)
                    .WithOne(u => u.Doctor)
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Specialty configurations
            modelBuilder.Entity<Specialty>(entity => 
            {
                entity.HasIndex(e => e.SpecialtyName).IsUnique();
            });

            // Appointment configurations
            modelBuilder.Entity<Appointment>(entity => 
            {
                entity.HasOne(a => a.Patient)
                    .WithMany(u => u.AppointmentsAsPatient)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.Property(a => a.Status).HasDefaultValue("Scheduled");
            });

            // Prescription configurations
            modelBuilder.Entity<Prescription>(entity => 
            {
                entity.HasOne(p => p.Appointment)
                    .WithOne(a => a.Prescription)
                    .HasForeignKey<Prescription>(p => p.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial data (optional but recommended for testing)
            SeedData(modelBuilder);
        }

        
        // Seed initial data for testing.
        // What for data seeding:
        // - Initial admin accounts
        // - Reference data (specialties)
        // - Test data in development
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Specialties
            modelBuilder.Entity<Specialty>().HasData(
                new Specialty { SpecialtyId = 1, SpecialtyName = "Cardiology", Description = "Heart and cardiovascular system", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 2, SpecialtyName = "Neurology", Description = "Brain and nervous system", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 3, SpecialtyName = "Pediatrics", Description = "Children's health", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 4, SpecialtyName = "Orthopedics", Description = "Bones, joints, and muscles", CreatedAt = DateTime.UtcNow },
                new Specialty { SpecialtyId = 5, SpecialtyName = "Dermatology", Description = "Skin, hair, and nails", CreatedAt = DateTime.UtcNow }
            );

            // Seed Admin User (Password: Admin@123)
            string adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            modelBuilder.Entity<User>().HasData(
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
