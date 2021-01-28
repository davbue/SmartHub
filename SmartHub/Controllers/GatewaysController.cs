using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHub.Models;

namespace SmartHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatewaysController : ControllerBase
    {
        private readonly SmartHomeContext _context;

        public GatewaysController(SmartHomeContext context)
        {
            _context = context;
        }

        // GET: api/Gateways
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gateway>>> GetGateways()
        {
            return await _context.Gateways.ToListAsync();
        }

        // GET: api/Gateways/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gateway>> GetGateway(long id)
        {
            var gateway = await _context.Gateways.FindAsync(id);
            
            if (gateway == null)
            {
                return NotFound();
            }
            return gateway;
        }

        // PUT: api/Gateways/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGateway(long id, Gateway gateway)
        {
            if (id != gateway.GatewayID)
            {
                return BadRequest();
            }

            _context.Entry(gateway).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GatewayExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Gateways
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Gateway>> PostGateway(Gateway gateway)
        {
            _context.Gateways.Add(gateway);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGateway", new { id = gateway.GatewayID }, gateway);
        }

        // DELETE: api/Gateways/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Gateway>> DeleteGateway(long id)
        {
            var gateway = await _context.Gateways.FindAsync(id);
            if (gateway == null)
            {
                return NotFound();
            }

            _context.Gateways.Remove(gateway);
            await _context.SaveChangesAsync();

            return gateway;
        }

        private bool GatewayExists(long id)
        {
            return _context.Gateways.Any(e => e.GatewayID == id);
        }
    }
}
