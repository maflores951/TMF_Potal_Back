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
            //var empleadoColumna = await _context.EmpleadoColumnas.FindAsync(id);

            //if (empleadoColumna == null)
            //{
            //    return NotFound();
            //}

            //return empleadoColumna;
            Respuesta respuesta = new Respuesta();

            var empleadoColumna = await _context.EmpleadoColumnas.
                                                                  Where(u => u.ConfiguracionSuaId == id ).
                                                                  FirstOrDefaultAsync();

            if (empleadoColumna == null)
            {
                respuesta.Exito = 0;
            }
            else
            {
                respuesta.Exito = 1;
            }

            return Ok(respuesta);
        }


        [HttpPost]
        [Route("ValidarColumnas")]
        public async Task<ActionResult<EmpleadoColumna>> ValidarColumnas(EmpleadoColumna empleadoColumnas)
        {
            Respuesta respuesta = new Respuesta();

            var empleadoColumna = await _context.EmpleadoColumnas.
                                                                  Where(u => u.ConfiguracionSuaId == empleadoColumnas.ConfiguracionSuaId && u.EmpleadoColumnaAnio == empleadoColumnas.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == empleadoColumnas.EmpleadoColumnaMes && u.ExcelTipoId == empleadoColumnas.ExcelTipoId).
                                                                  FirstOrDefaultAsync();

            if (empleadoColumna == null)
            {
                respuesta.Exito = 0;
            }
            else
            {
                respuesta.Exito = 1;
            }

            return Ok(respuesta);
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
                                var bandera = true;
                                //if(empleadoColumna != null)
                                //{
                                var excelColumnaModel = await _context.ExcelColumnas.
                                                                   Where(u => u.ExcelColumnaNombre == empleadoColumna.ExcelColumnaNombre && u.ExcelTipoId == empleadoColumna.ExcelTipoId).
                                                                   FirstOrDefaultAsync();

                                if (excelColumnaModel == null)
                                {
                                    //return BadRequest("No existe la columna: " + empleadoColumna.ExcelColumnaNombre + "en ninguna configuración.");
                                    bandera = false;
                                }

                                //if (bandera == true)
                                //{
                                //    var suaExcelModels = await _context.SuaExcels.
                                //                                       Where(u => u.ExcelColumnaId == excelColumnaModel.ExcelColumnaId).ToListAsync();

                                //    if (suaExcelModels == null)
                                //    {
                                //        //return BadRequest("No existe la columna en ninguna configuración.");
                                //        bandera = false;
                                //    }


                                //    foreach (var suaExcelModel in suaExcelModels)
                                //    {
                                //        var configuracionSuaNivelModel = await _context.ConfiguracionSuaNiveles.
                                //                                           Where(u => u.ConfiguracionSuaNivelId == suaExcelModel.ConfiguracionSuaNivelId && u.ConfiguracionSuaId == empleadoColumna.ConfiguracionSuaId).FirstOrDefaultAsync();

                                //        if (configuracionSuaNivelModel.ConfiguracionSuaId > 0)
                                //        {
                                //            //return BadRequest("No existe la columna en ninguna configuración.");
                                //            bandera = true;

                                //        }
                                //        else
                                //        {
                                //            bandera = false;
                                //        }
                                //    }
                                //}





                                ////empleadoColumna.ConfiguracionSuaId = empleadoColumna.ConfiguracionSuaId;
                                //if (bandera == true)
                                //{

                                empleadoColumna.ExcelColumnaId = excelColumnaModel.ExcelColumnaId;

                                var validaEmpleadoCoolumna = await _context.EmpleadoColumnas.
                                                                 Where(u => u.ConfiguracionSuaId == empleadoColumna.ConfiguracionSuaId && u.EmpleadoColumnaAnio == empleadoColumna.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == empleadoColumna.EmpleadoColumnaMes && u.ExcelTipoId == empleadoColumna.ExcelTipoId && u.ExcelColumnaId == empleadoColumna.ExcelColumnaId && u.EmpleadoColumnaNo == empleadoColumna.EmpleadoColumnaNo).
                                                                 FirstOrDefaultAsync();

                                if (validaEmpleadoCoolumna == null)
                                {
                                    _context.EmpleadoColumnas.Add(empleadoColumna);
                                }
                                else
                                {
                                    validaEmpleadoCoolumna.EmpleadoColumnaValor = empleadoColumna.EmpleadoColumnaValor;
                                    _context.Entry(validaEmpleadoCoolumna).State = EntityState.Modified;
                                }
                               
                                        //await _context.SaveChangesAsync();
                                        //transaction.Commit();
                                    //}
                                //}
                                



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

            _context.EmpleadoColumnas.RemoveRange(empleadoColumna);
            await _context.SaveChangesAsync();

            return empleadoColumna;
        }

        private bool EmpleadoColumnaExists(int id)
        {
            return _context.EmpleadoColumnas.Any(e => e.EmpleadoColumnaId == id);
        }
    }
}
