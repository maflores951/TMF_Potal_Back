using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Sua;
using LoginBase.Models.Response;
using Microsoft.AspNetCore.Authorization;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConfiguracionSuasController : ControllerBase
    {
        private readonly DataContext _context;

        //Se recupera el contexto para conectar con la base de datos
        public ConfiguracionSuasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ConfiguracionSuas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConfiguracionSua>>> GetConfiguracionSuas()
        {
            //Esta variable almacena la lista de configuraciones
            var responses = new List<ConfiguracionSua>();

            //Se recuperan las configuraciones en la base de datos
            var configuracionSuas = await _context.ConfiguracionSuas.ToListAsync();
            
            //Se recorren para poder validar que estan activos
            foreach (var configuracionSua in configuracionSuas)
            {
                if (configuracionSua.ConfSuaEstatus == false)
                {
                    //Se recuperan los niveles para asignarlos a cada registro
                    var modelConfiguracionSuaNiveles = await _context.ConfiguracionSuaNiveles.Where(d => d.ConfiguracionSuaId == configuracionSua.ConfiguracionSuaId).ToListAsync();

                    //Aqui se guardan los niveles por configuración
                    var suaNivel = new List<ConfiguracionSuaNivel>();

                    //Se recorren los niveles
                    foreach (var modelConfiguracionSuaNivel in modelConfiguracionSuaNiveles)
                    {
                        //Se recuperan las filas de cada configuración
                        var suaExcel = await _context.SuaExcels.Where(d => d.ConfiguracionSuaNivelId == modelConfiguracionSuaNivel.ConfiguracionSuaNivelId).ToListAsync();

                        //Se recorren las filas
                        foreach (var itemSua in suaExcel)
                        {
                            //Se recupera el tipo de excel por cada fila
                            var excelTipo = await _context.ExcelTipos.Where(d => d.ExcelTipoId == itemSua.ExcelTipoId).FirstOrDefaultAsync();

                            //Se recuperan los datos de la columna por cada excel
                            var excelColumna = await _context.ExcelColumnas.Where(d => d.ExcelColumnaId == itemSua.ExcelColumnaId).FirstOrDefaultAsync();
                            itemSua.ExcelTipo = excelTipo;
                            itemSua.ExcelColumna = excelColumna;
                        }
                       
                        //Se crean los niveles por cada configuración
                        suaNivel.Add(new ConfiguracionSuaNivel{
                            ConfiguracionSuaId = modelConfiguracionSuaNivel.ConfiguracionSuaId,
                            ConfiguracionSuaNivelId = modelConfiguracionSuaNivel.ConfiguracionSuaNivelId,
                            ConfSuaNNombre = modelConfiguracionSuaNivel.ConfSuaNNombre,
                            SuaExcel = suaExcel,
                        });
                    }

                    //Se agrega la configuración con toda su información relacionada
                    responses.Add(new ConfiguracionSua
                    {
                        ConfiguracionSuaId = configuracionSua.ConfiguracionSuaId,
                        ConfSuaNombre = configuracionSua.ConfSuaNombre,
                        ConfSuaEstatus = configuracionSua.ConfSuaEstatus,
                        ConfiguracionSuaTipo = configuracionSua.ConfiguracionSuaTipo,
                        ConfiguracionSuaNivel = suaNivel.ToList()
                    });
                }
            }
            //Se recuperan todas las configuraciones
            return Ok(responses);
        }

        // GET: api/ConfiguracionSuas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConfiguracionSua>> GetConfiguracionSua(int id)
        {
            var configuracionSua = await _context.ConfiguracionSuas.FindAsync(id);

            if (configuracionSua == null)
            {
                return NotFound();
            }

            return configuracionSua;
        }

        // PUT: api/ConfiguracionSuas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfiguracionSua(int id, ConfiguracionSua model)
        {
            //Se valida que el Id sea correcto
            if (id != model.ConfiguracionSuaId)
            {
                return BadRequest();
            }

            //Se prepara la tabla para realizar la actualización de la configuración
            _context.Entry(model).State = EntityState.Modified;

            //Se realiza la actualización
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfiguracionSuaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            //Se crea la variable que retorna la respuesta
            Respuesta respuesta = new Respuesta();

            //Se comienza a preparar la actualización de los niveles
            try
            {
                //Se asigna el contexto para conectar la base de datos
                using (DataContext db = _context)
                {
                    //Se crea una transaction para proteger la información con actualizaciones masivas
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Se recuperan los niveles con respecto a la configuración
                            var modelConfiguracionSuaNiveles = await _context.ConfiguracionSuaNiveles.Where(d => d.ConfiguracionSuaId == model.ConfiguracionSuaId).ToListAsync();
                            //Se recorren los niveles
                            foreach (var modelConfiguracionSuaNivel in modelConfiguracionSuaNiveles)
                            {
                                //Se recuperan las filas por cada nivel
                                var suaExceles = await _context.SuaExcels.Where(d => d.ConfiguracionSuaNivelId == modelConfiguracionSuaNivel.ConfiguracionSuaNivelId).ToListAsync();
                                //Se recorren las filas y se borran
                                foreach (var suaExcel in suaExceles)
                                {
                                    _context.SuaExcels.Remove(suaExcel);
                                    await _context.SaveChangesAsync();
                                }
                                //Se borran los niveles
                                _context.ConfiguracionSuaNiveles.Remove(modelConfiguracionSuaNivel);
                                await _context.SaveChangesAsync();
                            }
                            
                            //Se crea una variables para la configuración a guardar
                            var configuracionSua = new ConfiguracionSua();

                            //Se recorren los niveles y se agregan en la base de datos
                            foreach (var modelConfiguracionSuaNivel in model.ConfiguracionSuaNivel)
                            {
                                var configuracionSuaNivel = new ConfiguracionSuaNivel();
                                configuracionSuaNivel.ConfiguracionSuaId = id;
                                configuracionSuaNivel.ConfSuaNNombre = modelConfiguracionSuaNivel.ConfSuaNNombre;
                                db.ConfiguracionSuaNiveles.Add(configuracionSuaNivel);
                                db.SaveChanges();

                                //Se recorren las filas de los niveles y se agregan en la base de datos
                                foreach (var modelSuaExcel in modelConfiguracionSuaNivel.SuaExcel)
                                {
                                    var suaExcel = new SuaExcel();
                                    suaExcel.ConfiguracionSuaNivelId =              configuracionSuaNivel.ConfiguracionSuaNivelId;
                                    suaExcel.ExcelTipoId = modelSuaExcel.ExcelTipoId;
                                    suaExcel.ExcelColumnaId = modelSuaExcel.ExcelColumnaId;
                                    db.SuaExcels.Add(suaExcel);
                                    db.SaveChanges();
                                }
                            }
                            //Si todo es correcto se agrega a la base de datos
                            transaction.Commit();
                            respuesta.Exito = 1;
                        }
                        catch (Exception)
                        {
                            //Si algo esta mal se recupera la información y no se almacena la nueva información
                            transaction.Rollback();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                //Si algo sale mal no se hace el recorrido y regresa un error
                respuesta.Mensaje = ex.Message;
                respuesta.Exito = 0;
            }
            //S regresa la respuesta
            return Ok(respuesta);
        }

        // POST: api/ConfiguracionSuas
        [HttpPost]
        public async Task<ActionResult<ConfiguracionSua>> PostConfiguracionSua(ConfiguracionSua model)
        {
            var id = model.ConfiguracionSuaId;
           
            //Se valida que el id es correcto
            if (id != model.ConfiguracionSuaId)
            {
                return BadRequest();
            }

            //Se prepara la base de datos para actualizar la configuración
            _context.Entry(model).State = EntityState.Modified;

            //Si todo es correcto se guardan los datos
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfiguracionSuaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //Se crea las respuesta para retornarla
            Respuesta respuesta = new Respuesta();

            //Se elimina la infirmación relacionada para agregar la nueva.
            var respuestaE = await EliminarSuaNAsync(model);

            //var respuestaA = ActualizarSuaN(model);

            //Se valida que todo sea correcto
            if (respuestaE.Exito == 1 )
            {
                respuesta.Exito = 1;
                respuesta.Mensaje = "La información fue actualizada con exito";
            }
            else
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "Error en el servidor favor de contactar al administrador.";
            }

            //Se retorna la respuesta
            return Ok(respuesta);
        }

        // DELETE: api/ConfiguracionSuas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ConfiguracionSua>> DeleteConfiguracionSua(int id)
        {
            var configuracionSua = await _context.ConfiguracionSuas.FindAsync(id);
            if (configuracionSua == null)
            {
                return NotFound();
            }

            _context.ConfiguracionSuas.Remove(configuracionSua);
            await _context.SaveChangesAsync();

            return configuracionSua;
        }

        private bool ConfiguracionSuaExists(int id)
        {
            return _context.ConfiguracionSuas.Any(e => e.ConfiguracionSuaId == id);
        }

        private async Task<Respuesta> EliminarSuaNAsync(ConfiguracionSua model)
        {
            //Se crea la respuesta
            Respuesta respuesta = new Respuesta();
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
                            //Se recuperan los niveles con respecto a la configuración
                            var modelConfiguracionSuaNiveles = await _context.ConfiguracionSuaNiveles.Where(d => d.ConfiguracionSuaId == model.ConfiguracionSuaId).ToListAsync();
                            //Se recorren los niveles
                            foreach (var modelConfiguracionSuaNivel in modelConfiguracionSuaNiveles)
                            {
                                //Se recuperan las filas de las columnas
                                var suaExceles = await _context.SuaExcels.Where(d => d.ConfiguracionSuaNivelId == modelConfiguracionSuaNivel.ConfiguracionSuaNivelId).ToListAsync();
                                //Se recorren las filas de los niveles 
                                foreach (var suaExcel in suaExceles)
                                {
                                    //Se borran las filas de los niveles
                                    _context.SuaExcels.Remove(suaExcel);
                                    await _context.SaveChangesAsync();
                                }
                                //Se eliminan los niveles
                                _context.ConfiguracionSuaNiveles.Remove(modelConfiguracionSuaNivel);
                                await _context.SaveChangesAsync();
                            }
                            //Se guarda la eliminación
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
            return respuesta;
        }

        private Respuesta ActualizarSuaN(ConfiguracionSua model)
        {
            // Se recupera el id
            var id = model.ConfiguracionSuaId;

            //Se crear la variable de la respuesta
            Respuesta respuesta = new Respuesta();
            try
            {
                //Se recupera el contexto para conectar con la base de datos
                using (DataContext db = _context)
                {
                    //Se crea una transaction para proteger la base de datos
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Se crea la variable de la nueva configuración
                            var configuracionSua = new ConfiguracionSua();

                            //Se recorren los niveles por configuración
                            foreach (var modelConfiguracionSuaNivel in model.ConfiguracionSuaNivel)
                            {
                                //Se agregan los niveles en la base de datos
                                var configuracionSuaNivel = new ConfiguracionSuaNivel();
                                configuracionSuaNivel.ConfiguracionSuaId = id;
                                configuracionSuaNivel.ConfSuaNNombre = modelConfiguracionSuaNivel.ConfSuaNNombre;
                                db.ConfiguracionSuaNiveles.Add(configuracionSuaNivel);
                                db.SaveChanges();

                                //Se agregan las filas en cada nivel 
                                foreach (var modelSuaExcel in modelConfiguracionSuaNivel.SuaExcel)
                                {
                                    var suaExcel = new SuaExcel();
                                    suaExcel.ConfiguracionSuaNivelId = configuracionSuaNivel.ConfiguracionSuaNivelId;
                                    suaExcel.ExcelTipoId = modelSuaExcel.ExcelTipoId;
                                    suaExcel.ExcelColumnaId = modelSuaExcel.ExcelColumnaId;
                                    db.SuaExcels.Add(suaExcel);
                                    db.SaveChanges();
                                }
                            }
                            //Si todo es correcto se actualiza la base de datos
                            transaction.Commit();
                            respuesta.Exito = 1;
                        }
                        catch (Exception)
                        {
                            //Si algo es incorrecto se recupera la base de datos sin guardar registros erroneos
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Si algo sale mal retorna un error sin guardar información erronea
                respuesta.Mensaje = ex.Message;
                respuesta.Exito = 0;
            }
            //Se retorna la respuesta
            return respuesta;
        }
    }
}
