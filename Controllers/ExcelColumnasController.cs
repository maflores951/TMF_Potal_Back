using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Excel;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelColumnasController : ControllerBase
    {
        private readonly DataContext _context;

        public ExcelColumnasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ExcelColumnas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExcelColumna>>> GetExcelColumnas()
        {
            return await _context.ExcelColumnas.ToListAsync();
        }

        // GET: api/ExcelColumnas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExcelColumna>> GetExcelColumna(int id)
        {
            var excelColumna = await _context.ExcelColumnas.FindAsync(id);

            if (excelColumna == null)
            {
                return NotFound();
            }

            return excelColumna;
        }

        // PUT: api/ExcelColumnas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExcelColumna(int id, ExcelColumna excelColumna)
        {
            if (id != excelColumna.ExcelColumnaId)
            {
                return BadRequest();
            }

            _context.Entry(excelColumna).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExcelColumnaExists(id))
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

        // POST: api/ExcelColumnas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ExcelColumna>> PostExcelColumna(ExcelColumna excelColumna)
        {
            _context.ExcelColumnas.Add(excelColumna);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExcelColumna", new { id = excelColumna.ExcelColumnaId }, excelColumna);
        }

        // DELETE: api/ExcelColumnas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ExcelColumna>> DeleteExcelColumna(int id)
        {
            var excelColumna = await _context.ExcelColumnas.FindAsync(id);
            if (excelColumna == null)
            {
                return NotFound();
            }

            _context.ExcelColumnas.Remove(excelColumna);
            await _context.SaveChangesAsync();

            return excelColumna;
        }

        private bool ExcelColumnaExists(int id)
        {
            return _context.ExcelColumnas.Any(e => e.ExcelColumnaId == id);
        }
    }
}
