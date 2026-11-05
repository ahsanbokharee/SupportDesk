using SupportDesk.Server.Dtos;

namespace SupportDesk.Server.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(bool includeInactive);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<bool> PrefixExistsAsync(string prefix, int? exceptCategoryId = null);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);
}