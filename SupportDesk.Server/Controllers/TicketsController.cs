using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Services;

namespace SupportDesk.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<TicketDto>>> Get([FromQuery] TicketQuery query)
        {
            return Ok(await _ticketService.GetAllAsync(query));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TicketDto>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketDto>> Create(CreateTicketDto dto)
        {
            var categoryError = await _ticketService.ValidateCategoryAsync(dto.CategoryId);
            if (categoryError is not null)
            {
                return BadRequest(new { message = categoryError });
            }

            var ticket = await _ticketService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TicketDto>> Update(int id, UpdateTicketDto dto)
        {
            var categoryError = await _ticketService.ValidateCategoryAsync(dto.CategoryId, id);
            if (categoryError is not null)
            {
                return BadRequest(new { message = categoryError });
            }

            var ticket = await _ticketService.UpdateAsync(id, dto);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }
    }
}