using SupportDesk.Server.Models;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Services;

namespace SupportDesk.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> Get()
        {
            return Ok(await _ticketService.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }
        [HttpPost]
        public async Task<ActionResult<Ticket>> Create(CreateTicketDto dto)
        {

            var ticket = await _ticketService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Ticket>> Update(int id, UpdateTicketDto dto)
        {
            var ticket = await _ticketService.UpdateAsync(id, dto);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

    }
}
