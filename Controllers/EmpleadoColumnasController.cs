using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Empleado;
using LoginBase.Models.Response;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoColumnasController : ControllerBase
    {
        private readonly DataContext _context;

        public EmpleadoColumnasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/EmpleadoColumnas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpleadoColumna>>> GetEmpleadoColumnas()
        {
            return await _context.EmpleadoColumnas.ToListAsync();
        }

        // GET: api/EmpleadoColumnas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoColumna>> GetEmpleadoColumna(int id)
        {
            var empleadoColumna = await _context.EmpleadoColumnas.FindAsync(id);

            if (empleadoColumna == null)
            {
                return NotFound();
            }

            return empleadoColumna;
        }

        // PUT: api/EmpleadoColumnas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleadoColumna(int id, EmpleadoColumna empleadoColumna)
        {
            if (id != empleadoColumna.EmpleadoColumnaId)
            {
                return BadRequest();
            }

            _context.Entry(empleadoColumna).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoColumnaExists(id))
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

        // POST: api/EmpleadoColumnas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EmpleadoColumna>> PostEmpleadoColumna(List<EmpleadoColumna> empleadoColumnas)
        {
            Respuesta respuesta = new Respuesta();
           
            try
            {
                using (DataContext db = _context)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            

                            foreach (var empleadoColumna in empleadoColumnas)
                            {

                                //var excelColumnaModel = await _context.ExcelColumnas.
                                //   Where(u => u.ExcelColumnaNombre.ToLower() == empleadoColumna.ExcelColumnaNombre.ToLower() && u.ExcelTipoId == empleadoColumna.ExcelTipoId).
                                //   FirstOrDefaultAsync();

                                //if (excelColumnaModel == null)
                                //{
                                //    return BadRequest("No existe la columna en ninguna configuración.");
                                //}

                                //var suaExcelModel = await _context.SuaExcels.
                                //   Where(u => u.ExcelColumnaId == excelColumnaModel.ExcelColumnaId && u.ConfiguracionSuaNivel.ConfiguracionSuaId == empleadoColumna.ConfiguracionSuaId).
                                //   FirstOrDefaultAsync();

                                //if (suaExcelModel == null)
                                //{
                                //    return BadRequest("No existe la columna en ninguna configuración.");
                                //}

                                empleadoColumna.SuaExcelId = 80;//suaExcelModel.SuaExcelId;

                                _context.EmpleadoColumnas.Add(empleadoColumna);
                               

                                //CreatedAtAction("GetEmpleadoColumna", new { id = empleadoColumna.EmpleadoColumnaId }, empleadoColumna);
                            }
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        respuesta.Exito = 1;
                        }
                        catch (Exception)
                    {
                        respuesta.Mensaje = "Error";
                        respuesta.Exito = 0;
                        transaction.Rollback();
                    }

                }

                }
            }
            catch (Exception ex)
            {

                respuesta.Mensaje = ex.Message;
                respuesta.Exito = 0;
            }

            return Ok(respuesta);
        }

        // DELETE: api/EmpleadoColumnas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EmpleadoColumna>> DeleteEmpleadoColumna(int id)
        {
            var empleadoColumna = await _context.EmpleadoColumnas.FindAsync(id);
            if (empleadoColumna == null)
            {
                return NotFound();
            }

            _context.EmpleadoColumnas.Remove(empleadoColumna);
            await _context.SaveChangesAsync();

            return empleadoColumna;
        }

        private bool EmpleadoColumnaExists(int id)
        {
            return _context.EmpleadoColumnas.Any(e => e.EmpleadoColumnaId == id);
        }
    }
}
