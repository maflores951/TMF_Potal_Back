using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Menu;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolModulosController : ControllerBase
    {
        private readonly DataContext _context;

        public RolModulosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/RolModulos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolModulo>>> GetRolModulos()
        {
            return await _context.RolModulos.ToListAsync();
        }

        // GET: api/RolModulos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolModulo>> GetRolModulo(int id)
        {
            var rolModulo = await _context.RolModulos.FindAsync(id);

            if (rolModulo == null)
            {
                return NotFound();
            }

            return rolModulo;
        }

        // PUT: api/RolModulos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRolModulo(int id, RolModulo rolModulo)
        {
            if (id != rolModulo.RolModuloId)
            {
                return BadRequest();
            }

            _context.Entry(rolModulo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolModuloExists(id))
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

        // POST: api/RolModulos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RolModulo>> PostRolModulo(RolModulo rolModulo)
        {
            _context.RolModulos.Add(rolModulo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRolModulo", new { id = rolModulo.RolModuloId }, rolModulo);
        }

        // DELETE: api/RolModulos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RolModulo>> DeleteRolModulo(int id)
        {
            var rolModulo = await _context.RolModulos.FindAsync(id);
            if (rolModulo == null)
            {
                return NotFound();
            }

            _context.RolModulos.Remove(rolModulo);
            await _context.SaveChangesAsync();

            return rolModulo;
        }

        private bool RolModuloExists(int id)
        {
            return _context.RolModulos.Any(e => e.RolModuloId == id);
        }
    }
}
