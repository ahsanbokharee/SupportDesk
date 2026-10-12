using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Server.Models;

namespace SupportDesk.Server.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var users = new List<User>
        {
            new() { FirstName = "Ahsan", LastName = "Admin", Email = "admin@supportdesk.local", Role = "Admin" },
            new() { FirstName = "Sarah", LastName = "Agent", Email = "agent@supportdesk.local", Role = "Agent" },
            new() { FirstName = "John", LastName = "Customer", Email = "customer@supportdesk.local", Role = "Customer" },
        };

        var hasher = new PasswordHasher<User>();
        foreach (var user in users)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = hasher.HashPassword(user, "Password123!");
        }

        db.Users.AddRange(users);
        await db.SaveChangesAsync();
    }
}