using System;
using SupportDesk.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace SupportDesk.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TicketAlias> TicketAliases => Set<TicketAlias>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>()
            .Property(t => t.CreatedAt)
            .HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));


        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.Email).HasMaxLength(256);
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.Role).HasMaxLength(20);
            user.Property(u => u.CreatedAt).HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
        modelBuilder.Entity<Category>(category =>
        {
            category.Property(c => c.Name).HasMaxLength(100);
            category.Property(c => c.Prefix).HasMaxLength(6);
            category.HasIndex(c => c.Prefix).IsUnique();
            category.Property(c => c.DefaultPriority).HasMaxLength(10);
            category.Property(c => c.Color).HasMaxLength(7);
            category.Property(c => c.CreatedAt).HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
        modelBuilder.Entity<Ticket>(ticket =>
        {
            ticket.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // No (CategoryId, Number) index: TicketNumber's unique index already guarantees uniqueness,
            // because prefixes are unique and locked once used. Moved tickets are re-keyed
            // (they take the next number in the new category); their old key goes to TicketAliases.
            //ticket.HasIndex(t => new { t.CategoryId, t.Number }).IsUnique();

            ticket.Property(t => t.TicketNumber).HasMaxLength(20);
            ticket.HasIndex(t => t.TicketNumber).IsUnique();
        });
        modelBuilder.Entity<TicketAlias>(alias =>
        {
            alias.HasOne(a => a.Ticket)
                 .WithMany(t => t.Aliases)
                 .HasForeignKey(a => a.TicketId)
                 .OnDelete(DeleteBehavior.Cascade);

            alias.HasOne<Category>()
                .WithMany()
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            alias.HasIndex(a => new { a.TicketId, a.CategoryId }).IsUnique();

            alias.Property(a => a.TicketNumber).HasMaxLength(20);
            alias.HasIndex(a => a.TicketNumber).IsUnique();

            alias.Property(a => a.CreatedAt).HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
    }
}
