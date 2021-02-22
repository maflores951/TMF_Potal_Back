using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosController : ControllerBase
    {
        private readonly DataContext _context;

        public ParametrosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Parametros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parametro>>> GetParametros()
        {
            var responses = new List<Parametro>();

            var parametros = await _context.Parametros.ToListAsync();

            foreach (var parametro in parametros)
            {
                if (parametro.ParametroEstatusDelete == false)
                {
                    responses.Add(new Parametro
                    {
                        ParametroId = parametro.ParametroId,
                        ParametroNombre = parametro.ParametroNombre,
                        ParametroClave = parametro.ParametroClave,
                        ParametroDescripcion = parametro.ParametroDescripcion,
                        ParametroValorInicial = parametro.ParametroValorInicial,
                        ParametroValorFinal = parametro.ParametroValorFinal,
                        ParametroEstatusDelete = parametro.ParametroEstatusDelete
                    });
                }
            }
            //return await _context.Parametros.ToListAsync();
            return Ok(responses);
        }

        // GET: api/Parametros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Parametro>> GetParametro(int id)
        {
            var responses = new Parametro();
            var parametro = await _context.Parametros.FindAsync(id);

            if (parametro == null)
            {
                return NotFound();
            }

            responses = (new Parametro
            {
                ParametroId = parametro.ParametroId,
                ParametroNombre = parametro.ParametroNombre,
                ParametroClave = parametro.ParametroClave,
                ParametroDescripcion = parametro.ParametroDescripcion,
                ParametroValorInicial = parametro.ParametroValorInicial,
                ParametroValorFinal = parametro.ParametroValorFinal,
                ParametroEstatusDelete = parametro.ParametroEstatusDelete
            });

            //return parametro;
            return Ok(responses);
        }

        // PUT: api/Parametros/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParametro(int id, Parametro parametro)
        {
            if (id != parametro.ParametroId)
            {
                return BadRequest();
            }

            _context.Entry(parametro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParametroExists(id))
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

        // POST: api/Parametros
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Parametro>> PostParametro(Parametro parametro)
        {
            var parametroClave = string.Empty;
            _context.Parametros.Add(parametro);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParametro", new { id = parametro.ParametroId }, parametro);
        }

        // DELETE: api/Parametros/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Parametro>> DeleteParametro(int id)
        {
            var parametro = await _context.Parametros.FindAsync(id);
            if (parametro == null)
            {
                return NotFound();
            }

            _context.Parametros.Remove(parametro);
            await _context.SaveChangesAsync();

            return parametro;
        }

        private bool ParametroExists(int id)
        {
            return _context.Parametros.Any(e => e.ParametroId == id);
        }
    }
}
