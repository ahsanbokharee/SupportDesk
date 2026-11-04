using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Services;

namespace SupportDesk.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]                                   // any logged-in user can READ categories (ticket form needs them)
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        // Only admins may see deactivated categories
        var showInactive = includeInactive && User.IsInRole("Admin");
        return Ok(await _categoryService.GetAllAsync(showInactive));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
    {
        if (await _categoryService.PrefixExistsAsync(dto.Prefix))
        {
            return Conflict(new { message = $"The prefix {dto.Prefix} is already used by another category." });
        }

        var category = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryDto dto)
    {
        var existing = await _categoryService.GetByIdAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.Prefix != dto.Prefix && existing.TicketsIssued > 0)
        {
            return BadRequest(new { message = "The prefix can't change once the category has tickets." });
        }

        if (existing.IsDefault && !dto.IsActive)
        {
            return BadRequest(new { message = "The default category can't be deactivated." });
        }

        if (await _categoryService.PrefixExistsAsync(dto.Prefix, id))
        {
            return Conflict(new { message = $"The prefix {dto.Prefix} is already used by another category." });
        }

        return Ok(await _categoryService.UpdateAsync(id, dto));
    }
}