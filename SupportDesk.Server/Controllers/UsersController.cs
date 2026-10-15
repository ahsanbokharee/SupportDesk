using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Services;

namespace SupportDesk.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        return Ok( await _userService.GetAllAsync());
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = _userService.GetByIdAsync(id);
        
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        if (await _userService.EmailExistsAsync(dto.Email))
        {
            return Conflict(new { message = "A user with this email already exists." });
        }   
        var user = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById),new { id = user.Id }, user);
    }
    [HttpPut]
    public async Task<ActionResult<UserDto>> Update(int id, UpdateUserDto dto)
    {
        var currentUserId = int.Parse(User.FindFirstValue("sub")!);
        if (id == currentUserId && (!dto.IsActive || dto.Role != "Admin"))
        {
            return BadRequest(new { message = "You can't deactivate yourself or remove your own admin role." });
        }
        if (await _userService.EmailExistsAsync(dto.Email, exceptUserId: id))
        {
            return Conflict(new { message = "A user with this email already exists." });
        }

        var user = await _userService.UpdateAsync(id, dto);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}