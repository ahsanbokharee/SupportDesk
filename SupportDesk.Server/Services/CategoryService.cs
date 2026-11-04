using Microsoft.EntityFrameworkCore;
using SupportDesk.Server.Data;
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Models;

namespace SupportDesk.Server.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> GetAllAsync(bool includeInactive)
    {
        var categories = _db.Categories.AsQueryable();
        if (!includeInactive)
        {
            categories = categories.Where(c => c.IsActive);
        }

        return await categories
            .OrderByDescending(c => c.IsDefault)
            .ThenBy(c => c.Name)
            .Select(c => ToDto(c))
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        return category is null ? null : ToDto(category);
    }

    public Task<bool> PrefixExistsAsync(string prefix, int? exceptCategoryId = null)
    {
        return _db.Categories.AnyAsync(c => c.Prefix == prefix && c.Id != exceptCategoryId);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Prefix = dto.Prefix,
            DefaultPriority = dto.DefaultPriority,
            Color = dto.Color,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return ToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null)
        {
            return null;
        }

        category.Name = dto.Name.Trim();
        category.Prefix = dto.Prefix;
        category.DefaultPriority = dto.DefaultPriority;
        category.Color = dto.Color;
        category.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        return ToDto(category);
    }

    private static CategoryDto ToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Prefix = c.Prefix,
        DefaultPriority = c.DefaultPriority,
        Color = c.Color,
        IsActive = c.IsActive,
        IsDefault = c.IsDefault,
        TicketsIssued = c.LastNumber,
    };
}