using SupportDesk.Server.Data;
using SupportDesk.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Server.Dtos;

namespace SupportDesk.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TicketsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> Get()
        {
            var tickets = await _db.Tickets
                    .OrderBy(t => t.Id)
                    .ToListAsync();


            return Ok(tickets);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<Ticket>> GetById(int Id)
        {
            var ticket = await _db.Tickets.FindAsync(Id);
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }
        [HttpPost]
        public async Task<ActionResult<Ticket>> Create(CreateTicketDto dto)
        {
            var ticket = new Ticket
            {
                Subject = dto.Subject,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };
            
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            ticket.TicketNumber = $"TCK-{1000 + ticket.Id}";
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new {id = ticket.Id}, ticket);
        }

    }
}
