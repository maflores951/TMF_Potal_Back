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
    public class FuncionsController : ControllerBase
    {
        private readonly DataContext _context;

        public FuncionsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Funcions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Funcion>>> GetFunciones()
        {
            return await _context.Funciones.ToListAsync();
        }

        // GET: api/Funcions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Funcion>> GetFuncion(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);

            if (funcion == null)
            {
                return NotFound();
            }

            return funcion;
        }

        // PUT: api/Funcions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFuncion(int id, Funcion funcion)
        {
            if (id != funcion.FuncionId)
            {
                return BadRequest();
            }

            _context.Entry(funcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FuncionExists(id))
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

        // POST: api/Funcions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Funcion>> PostFuncion(Funcion funcion)
        {
            _context.Funciones.Add(funcion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFuncion", new { id = funcion.FuncionId }, funcion);
        }

        // DELETE: api/Funcions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Funcion>> DeleteFuncion(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);
            if (funcion == null)
            {
                return NotFound();
            }

            _context.Funciones.Remove(funcion);
            await _context.SaveChangesAsync();

            return funcion;
        }

        private bool FuncionExists(int id)
        {
            return _context.Funciones.Any(e => e.FuncionId == id);
        }
    }
}
