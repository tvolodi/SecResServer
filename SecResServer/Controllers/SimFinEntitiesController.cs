using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecResServer.Model;
using SecResServer.Model.SimFin;

namespace SecResServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimFinEntitiesController : ControllerBase
    {
        private readonly SecResDbContext _context;

        public SimFinEntitiesController(SecResDbContext context)
        {
            _context = context;
        }

        // GET: api/SimFinEntities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SimFinEntity>>> GetsimFinEntities()
        {
            return await _context.simFinEntities.ToListAsync();
        }

        // GET: api/SimFinEntities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SimFinEntity>> GetSimFinEntity(int id)
        {
            var simFinEntity = await _context.simFinEntities.FindAsync(id);

            if (simFinEntity == null)
            {
                return NotFound();
            }

            return simFinEntity;
        }

        // PUT: api/SimFinEntities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSimFinEntity(int id, SimFinEntity simFinEntity)
        {
            if (id != simFinEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(simFinEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SimFinEntityExists(id))
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

        // POST: api/SimFinEntities
        [HttpPost]
        public async Task<ActionResult> PostSimFinEntity(string command)
        {
            switch (command)
            {
                case "ImportAll":


                    break;
                default:
                    break;
            }

            return NoContent();
        }

        // DELETE: api/SimFinEntities/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SimFinEntity>> DeleteSimFinEntity(int id)
        {
            var simFinEntity = await _context.simFinEntities.FindAsync(id);
            if (simFinEntity == null)
            {
                return NotFound();
            }

            _context.simFinEntities.Remove(simFinEntity);
            await _context.SaveChangesAsync();

            return simFinEntity;
        }

        private bool SimFinEntityExists(int id)
        {
            return _context.simFinEntities.Any(e => e.Id == id);
        }
    }
}
