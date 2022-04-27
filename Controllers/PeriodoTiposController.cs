using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using tmf_group.Models;

namespace tmf_group.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodoTiposController : ControllerBase
    {
        private readonly DataContext _context;

        public PeriodoTiposController(DataContext context)
        {
            _context = context;
        }

        // GET: api/PeriodoTipos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodoTipo>>> GetPeriodoTipos()
        {
            return await _context.PeriodoTipos.ToListAsync();
        }

        // GET: api/PeriodoTipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PeriodoTipo>> GetPeriodoTipo(int id)
        {
            var periodoTipo = await _context.PeriodoTipos.FindAsync(id);

            if (periodoTipo == null)
            {
                return NotFound();
            }

            return periodoTipo;
        }

        // PUT: api/PeriodoTipos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPeriodoTipo(int id, PeriodoTipo periodoTipo)
        {
            if (id != periodoTipo.PeriodoTipoId)
            {
                return BadRequest();
            }

            _context.Entry(periodoTipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeriodoTipoExists(id))
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

        // POST: api/PeriodoTipos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PeriodoTipo>> PostPeriodoTipo(PeriodoTipo periodoTipo)
        {
            _context.PeriodoTipos.Add(periodoTipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPeriodoTipo", new { id = periodoTipo.PeriodoTipoId }, periodoTipo);
        }

        // DELETE: api/PeriodoTipos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePeriodoTipo(int id)
        {
            var periodoTipo = await _context.PeriodoTipos.FindAsync(id);
            if (periodoTipo == null)
            {
                return NotFound();
            }

            _context.PeriodoTipos.Remove(periodoTipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PeriodoTipoExists(int id)
        {
            return _context.PeriodoTipos.Any(e => e.PeriodoTipoId == id);
        }
    }
}
