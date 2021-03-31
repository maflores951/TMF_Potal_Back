using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Excel;
using Microsoft.AspNetCore.Authorization;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ExcelTiposController : ControllerBase
    {
        private readonly DataContext _context;

        public ExcelTiposController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ExcelTipos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExcelTipo>>> GetExcelTipos()
        {
            return await _context.ExcelTipos.ToListAsync();
        }

        // GET: api/ExcelTipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExcelTipo>> GetExcelTipo(int id)
        {
            var excelTipo = await _context.ExcelTipos.FindAsync(id);

            if (excelTipo == null)
            {
                return NotFound();
            }

            return excelTipo;
        }

        // PUT: api/ExcelTipos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExcelTipo(int id, ExcelTipo excelTipo)
        {
            if (id != excelTipo.ExcelTipoId)
            {
                return BadRequest();
            }

            _context.Entry(excelTipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExcelTipoExists(id))
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

        // POST: api/ExcelTipos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ExcelTipo>> PostExcelTipo(ExcelTipo excelTipo)
        {
            _context.ExcelTipos.Add(excelTipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExcelTipo", new { id = excelTipo.ExcelTipoId }, excelTipo);
        }

        //// DELETE: api/ExcelTipos/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ExcelTipo>> DeleteExcelTipo(int id)
        //{
        //    var excelTipo = await _context.ExcelTipos.FindAsync(id);
        //    if (excelTipo == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ExcelTipos.Remove(excelTipo);
        //    await _context.SaveChangesAsync();

        //    return excelTipo;
        //}

        private bool ExcelTipoExists(int id)
        {
            return _context.ExcelTipos.Any(e => e.ExcelTipoId == id);
        }
    }
}
