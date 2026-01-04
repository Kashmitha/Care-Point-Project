using Microsoft.EntityFrameworkCore;
using CarePoint.API.Models;

namespace CarePoint.API.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

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

        /// <summary>
        /// Configure entity relationships and constraints.
        /// SQL Server doesn't allow multiple cascade paths.
        /// We use Restrict on some relationships to prevent cascade cycles.
        /// </summary>
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
                
                // One-to-One: User -> Doctor (Cascade delete is OK here)
                entity.HasOne(d => d.User)
                    .WithOne(u => u.Doctor)
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One: Doctor -> Specialty (Restrict - don't delete doctors when specialty is deleted)
                entity.HasOne(d => d.Specialty)
                    .WithMany(s => s.Doctors)
                    .HasForeignKey(d => d.SpecialtyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Specialty configurations
            modelBuilder.Entity<Specialty>(entity => 
            {
                entity.HasIndex(e => e.SpecialtyName).IsUnique();
            });

            // Appointment configurations
            modelBuilder.Entity<Appointment>(entity => 
            {
                // Many-to-One: Appointment -> Patient (Restrict to prevent cascade)
                entity.HasOne(a => a.Patient)
                    .WithMany(u => u.AppointmentsAsPatient)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: Appointment -> Doctor (Restrict to prevent cascade)
                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.Property(a => a.Status).HasDefaultValue("Scheduled");
            });

            // Prescription configurations
            modelBuilder.Entity<Prescription>(entity => 
            {
                // One-to-One: Appointment -> Prescription (Cascade is OK)
                entity.HasOne(p => p.Appointment)
                    .WithOne(a => a.Prescription)
                    .HasForeignKey<Prescription>(p => p.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One: Prescription -> Patient (Restrict - FIXES CASCADE CYCLE)
                entity.HasOne(p => p.Patient)
                    .WithMany()
                    .HasForeignKey(p => p.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: Prescription -> Doctor (Restrict - FIXES CASCADE CYCLE)
                entity.HasOne(p => p.Doctor)
                    .WithMany(d => d.Prescriptions)
                    .HasForeignKey(p => p.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PrescriptionMedication configurations
            modelBuilder.Entity<PrescriptionMedication>(entity =>
            {
                // Many-to-One: PrescriptionMedication -> Prescription (Cascade is OK)
                entity.HasOne(pm => pm.Prescription)
                    .WithMany(p => p.Medications)
                    .HasForeignKey(pm => pm.PrescriptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MedicalRecord configurations
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                // Many-to-One: MedicalRecord -> Patient (Restrict)
                entity.HasOne(mr => mr.Patient)
                    .WithMany()
                    .HasForeignKey(mr => mr.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: MedicalRecord -> Doctor (Restrict)
                entity.HasOne(mr => mr.Doctor)
                    .WithMany()
                    .HasForeignKey(mr => mr.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: MedicalRecord -> Appointment (Restrict)
                entity.HasOne(mr => mr.Appointment)
                    .WithMany(a => a.MedicalRecords)
                    .HasForeignKey(mr => mr.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification configurations
            modelBuilder.Entity<Notification>(entity =>
            {
                // Many-to-One: Notification -> User (Cascade is OK)
                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One: Notification -> Appointment (Restrict)
                entity.HasOne(n => n.RelatedAppointment)
                    .WithMany()
                    .HasForeignKey(n => n.RelatedAppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Review configurations
            modelBuilder.Entity<Review>(entity =>
            {
                // Many-to-One: Review -> Doctor (Restrict)
                entity.HasOne(r => r.Doctor)
                    .WithMany(d => d.Reviews)
                    .HasForeignKey(r => r.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: Review -> Patient (Restrict)
                entity.HasOne(r => r.Patient)
                    .WithMany()
                    .HasForeignKey(r => r.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Many-to-One: Review -> Appointment (Restrict)
                entity.HasOne(r => r.Appointment)
                    .WithMany()
                    .HasForeignKey(r => r.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DoctorAvailability configurations
            modelBuilder.Entity<DoctorAvailability>(entity =>
            {
                // Many-to-One: DoctorAvailability -> Doctor (Cascade is OK)
                entity.HasOne(da => da.Doctor)
                    .WithMany(d => d.Availabilities)
                    .HasForeignKey(da => da.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // NO SEEDING HERE - Keep migrations clean
        }
    }
}