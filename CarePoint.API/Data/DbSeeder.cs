using Microsoft.EntityFrameworkCore;
using CarePoint.API.Models;

namespace CarePoint.API.Data
{
    /// <summary>
    /// Database seeder for runtime data initialization.
    /// INTERVIEW TIP: Separating seeding from migrations keeps migrations clean
    /// and allows for flexible data initialization based on environment.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Ensure database is created
                await context.Database.MigrateAsync();

                // Seed Specialties
                if (!await context.Specialties.AnyAsync())
                {
                    logger.LogInformation("Seeding specialties...");
                    
                    var specialties = new List<Specialty>
                    {
                        new Specialty { SpecialtyName = "Cardiology", Description = "Heart and cardiovascular system" },
                        new Specialty { SpecialtyName = "Neurology", Description = "Brain and nervous system" },
                        new Specialty { SpecialtyName = "Pediatrics", Description = "Children's health" },
                        new Specialty { SpecialtyName = "Orthopedics", Description = "Bones, joints, and muscles" },
                        new Specialty { SpecialtyName = "Dermatology", Description = "Skin, hair, and nails" }
                    };

                    await context.Specialties.AddRangeAsync(specialties);
                    await context.SaveChangesAsync();
                    
                    logger.LogInformation("Specialties seeded successfully");
                }

                // Seed Admin User
                if (!await context.Users.AnyAsync(u => u.Email == "admin@carepoint.com"))
                {
                    logger.LogInformation("Seeding admin user...");
                    
                    var adminUser = new User
                    {
                        Email = "admin@carepoint.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        FirstName = "System",
                        LastName = "Administrator",
                        Role = "Admin",
                        IsActive = true
                    };

                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();
                    
                    logger.LogInformation("Admin user seeded successfully");
                }

                logger.LogInformation("Database seeding completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}