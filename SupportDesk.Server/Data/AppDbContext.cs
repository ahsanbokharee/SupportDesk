using System;
using SupportDesk.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace SupportDesk.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options) {}
    public DbSet<Ticket> Tickets => Set<Ticket>();
}
