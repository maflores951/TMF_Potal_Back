using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Empleado;
using LoginBase.Models.Response;
using System.Threading;
using Microsoft.AspNetCore.Authorization;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

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
            //Se recuperan todos los empleados de la base de datos
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

        //Api para validar si existen resultados con respecto a un año y un mes
        [HttpPost]
        [Route("ValidarColumnas")]
        public async Task<ActionResult<EmpleadoColumna>> ValidarColumnas(EmpleadoColumna empleadoColumnas)
        {
            //Se crea la respuesta
            Respuesta respuesta = new Respuesta();

            //Se busca si existe un empleado con respecto a el año, mes, tipo de archivo y usuario.
            var empleadoColumna = await _context.EmpleadoColumnas.
                                                                 Where(u =>  u.EmpleadoColumnaAnio == empleadoColumnas.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == empleadoColumnas.EmpleadoColumnaMes && u.ExcelTipoId == empleadoColumnas.ExcelTipoId && u.UsuarioId == empleadoColumnas.UsuarioId).FirstOrDefaultAsync();

            //Se valida el resultado 
            if (empleadoColumna == null)
            {
                respuesta.Exito = 0;
            }
            else
            {
                respuesta.Exito = 1;
            }
            //Se recupera la respuesta
            return Ok(respuesta);
        }

        //Api para eliminar las columnas y optimizar los tiempos de actualización de datos 
        [HttpPost]
        [Route("EliminarColumnas")]
        public async Task<ActionResult<EmpleadoColumna>> EliminarColumnas(EmpleadoColumna empleadoColumna)
        {
            //Se crea la respuesta
            Respuesta respuesta = new Respuesta();

            //Se busca si existe un empleado con respecto a el año, mes, tipo de archivo y usuario.
            var empleadoColumnas = await _context.EmpleadoColumnas.
                                                                 Where(u => u.EmpleadoColumnaAnio == empleadoColumna.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == empleadoColumna.EmpleadoColumnaMes && u.ExcelTipoId == empleadoColumna.ExcelTipoId && u.UsuarioId == empleadoColumna.UsuarioId).ToListAsync();

            //Se valida el resultado 
            if (empleadoColumna == null)
            {
                respuesta.Exito = 0;
            }
            else
            {
                try
                {
                    //Se recupera el contexto para conectar con la base de datos
                    using (DataContext db = _context)
                    {
                        //Se crea una transacción para poder proteger los datos
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                               
                                //Se recorren los niveles
                                foreach (var empleadoCol in empleadoColumnas)
                                {
                                   
                                    //Se eliminan los niveles
                                    _context.EmpleadoColumnas.Remove(empleadoCol);
                                    
                                }
                                //Se guarda la eliminación
                                await _context.SaveChangesAsync();
                                transaction.Commit();
                                respuesta.Exito = 1;
                            }
                            catch (Exception)
                            {
                                //Si algo sale mal se restaura la base de datos sin almacenar los errores
                                transaction.Rollback();
                                respuesta.Exito = 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si algo sale mal no se ejecuta el flujo y retorna un error
                    respuesta.Mensaje = ex.Message;
                    respuesta.Exito = 0;
                }
            }


            //Se recupera la respuesta
            return Ok(respuesta);
        }

        // PUT: api/EmpleadoColumnas/5
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
        [HttpPost]
        public async Task<ActionResult<EmpleadoColumna>> PostEmpleadoColumna(List<EmpleadoColumna> empleadoColumnas)
        {
            //Se crea la respuesta
            Respuesta respuesta = new Respuesta();
           

            try
            {
                //Se crea el contexto para conectar con la base de datos
                using (DataContext db = _context)
                {
                    //Se crea una transaction para proteger la base de datos
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Se crea esta variable para poder enviar la información por lotes y optimizar el performance
                            var lote = 0;
                            var banderaUpdate = true;
                            var banderaValida = true;
                            //Se recorren los datos por empleado para guardarlos
                            foreach (var empleadoColumna in empleadoColumnas)
                            {
                                //Se suma cada registro
                                lote++;
                                var bandera = true;
                                //Se recuperan los datos de la columna del excel relacionado
                                var excelColumnaModel = await _context.ExcelColumnas.
                                                                   Where(u => u.ExcelColumnaNombre == empleadoColumna.ExcelColumnaNombre && u.ExcelTipoId == empleadoColumna.ExcelTipoId).
                                                                   FirstOrDefaultAsync();
                                //Se valida si existe la columna 
                                if (excelColumnaModel == null)
                                {
                                    bandera = false;
                                }

                                //Si no existe la columna no se guarda en la base de datos
                                if (bandera == true)
                                {
                                    //Se asigna el id de la columna a cada registro
                                    empleadoColumna.ExcelColumnaId = excelColumnaModel.ExcelColumnaId;

                                    //if (banderaUpdate == true && banderaValida == true)
                                    //{
                                    //    // Se busca si ya existe la columna
                                    //var updateEmpleadoCoolumna = await _context.EmpleadoColumnas.
                                    //                                 Where(u => u.EmpleadoColumnaAnio == empleadoColumna.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == empleadoColumna.EmpleadoColumnaMes && u.UsuarioId == empleadoColumna.UsuarioId).
                                    //                                 FirstOrDefaultAsync();
                                    //    if (updateEmpleadoCoolumna == null)
                                    //    {
                                    //        banderaUpdate = false;
                                    //    }
                                    //    banderaValida = false;
                                    //}

                                    var validaEmpleadoCoolumna = new EmpleadoColumna();
                                    //Se busca si ya existe la columna
                                    //if (banderaUpdate == true)
                                    //{
                                    //    validaEmpleadoCoolumna = await _context.EmpleadoColumnas.
                                    //                                 Where(u => u.EmpleadoColumnaAnio == empleadoColumna.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == empleadoColumna.EmpleadoColumnaMes && u.ExcelTipoId == empleadoColumna.ExcelTipoId && u.ExcelColumnaId == empleadoColumna.ExcelColumnaId && u.EmpleadoColumnaNo == empleadoColumna.EmpleadoColumnaNo && u.UsuarioId == empleadoColumna.UsuarioId).
                                    //                                 FirstOrDefaultAsync();
                                    //}
                                    //else
                                    //{
                                    //    validaEmpleadoCoolumna = null;
                                    //}
                                    //Si no existe se guarda
                                    //if (validaEmpleadoCoolumna == null)
                                    //{
                                        _context.EmpleadoColumnas.Add(empleadoColumna);
                                    //}
                                    ////Si existe se actualiza
                                    //else
                                    //{
                                    //    validaEmpleadoCoolumna.EmpleadoColumnaValor = empleadoColumna.EmpleadoColumnaValor;
                                    //    _context.Entry(validaEmpleadoCoolumna).State = EntityState.Modified;
                                    //}

                                    //Cada 1000 registros se actualiza la base de datos
                                    if (lote == 1000)
                                    {
                                        await _context.SaveChangesAsync();
                                        lote = 0;
                                    }
                                }
                            }
                            //Se guardan los ultimos registros y se confirman los registros
                            await _context.SaveChangesAsync();
                            transaction.Commit();
                            respuesta.Exito = 1;
                        }
                        catch (Exception)
                        {
                            //Si algo sale mal no se actualiza la base de datos 
                            respuesta.Mensaje = "Error";
                            respuesta.Exito = 0;
                            transaction.Rollback();
                        }

                    }

                }
            }
            //Si algo sale mal no se ejecutan los registros
            catch (Exception ex)
            {
                respuesta.Mensaje = ex.Message;
                respuesta.Exito = 0;
            }

            //Se recupera la respuesta
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
