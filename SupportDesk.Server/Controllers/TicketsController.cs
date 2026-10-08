using SupportDesk.Server.Data;
using SupportDesk.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    }
}
