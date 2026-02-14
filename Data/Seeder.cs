using net_api.Models;
using Microsoft.EntityFrameworkCore;

namespace net_api.Data
{
    public static class Seeder
    {
        public static async Task SeedAdminUser(ApplicationDbContext context)
        {
            // Check if admin user already exists
            var existingAdmin = await context.Users
                .FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");

            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@gmail.com",
                    Password = "admin12345678", // In production, hash this password!
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                Console.WriteLine("Admin user seeded successfully:");
                Console.WriteLine("Email: admin@gmail.com");
                Console.WriteLine("Password: admin12345678");
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }
    }
}
