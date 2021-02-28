using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Sua;
using LoginBase.Models.Response;
using LoginBase.Services;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionSuasController : ControllerBase
    {
        private readonly DataContext _context;

        //private readonly SuaService _suaService;

        public ConfiguracionSuasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ConfiguracionSuas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConfiguracionSua>>> GetConfiguracionSuas()
        {
            //return await _context.ConfiguracionSuas.ToListAsync();
            var responses = new List<ConfiguracionSua>();

            var configuracionSuas = await _context.ConfiguracionSuas.ToListAsync();
            
            foreach (var configuracionSua in configuracionSuas)
            {
                if (configuracionSua.ConfSuaEstatus == false)
                {
                    var modelConfiguracionSuaNiveles = await _context.ConfiguracionSuaNiveles.Where(d => d.ConfiguracionSuaId == configuracionSua.ConfiguracionSuaId).ToListAsync();

                    var suaNivel = new List<ConfiguracionSuaNivel>();


                    foreach (var modelConfiguracionSuaNivel in modelConfiguracionSuaNiveles)
                    {
                        var suaExcel = await _context.SuaExcels.Where(d => d.ConfiguracionSuaNivelId == modelConfiguracionSuaNivel.ConfiguracionSuaNivelId).ToListAsync();

                        foreach (var itemSua in suaExcel)
                        {
                            var excelTipo = await _context.ExcelTipos.Where(d => d.ExcelTipoId == itemSua.TipoPeriodoId).FirstOrDefaultAsync();

                            var excelColumna = await _context.ExcelColumnas.Where(d => d.ExcelColumnaId == itemSua.ExcelColumnaId).FirstOrDefaultAsync();
                            itemSua.ExcelTipo = excelTipo;
                            itemSua.ExcelColumna = excelColumna;
                        }
                       

                        suaNivel.Add(new ConfiguracionSuaNivel{
                            ConfiguracionSuaId = modelConfiguracionSuaNivel.ConfiguracionSuaId,
                            ConfiguracionSuaNivelId = modelConfiguracionSuaNivel.ConfiguracionSuaNivelId,
                            ConfSuaNNombre = modelConfiguracionSuaNivel.ConfSuaNNombre,
                            SuaExcel = suaExcel,
                        });
                    }

                    responses.Add(new ConfiguracionSua
                    {
                        ConfiguracionSuaId = configuracionSua.ConfiguracionSuaId,
                        ConfSuaNombre = configuracionSua.ConfSuaNombre,
                        ConfSuaEstatus = configuracionSua.ConfSuaEstatus,
                        ConfiguracionSuaNivel = suaNivel.ToList()//configuracionSua.ConfiguracionSuaNivel.ToList()
                    });
                }
            }
            return Ok(responses);

            //var responses = new List<ConfiguracionSuaNivel>();

            //var configuracionSuaNiveles = await _context.ConfiguracionSuaNiveles.ToListAsync();

            //foreach (var configuracionSuaNivel in configuracionSuaNiveles)
            //{
            //if (configuracionSuaNivel.ConfiguracionSua.ConfSuaEstatus == false)
            //{
            //var modelConfiguracionSuaNivel = await _context.ConfiguracionSuaNiveles.Where(d => d.ConfiguracionSuaId == configuracionSua.ConfiguracionSuaId).ToListAsync();

            //var configuracionSua = await _context.ConfiguracionSuas.FindAsync(configuracionSuaNivel.ConfiguracionSuaId);
            //responses.Add(new ConfiguracionSuaNivel
            //    {
            //        ConfiguracionSuaNivelId = configuracionSuaNivel.ConfiguracionSuaNivelId,
            //        ConfSuaNNombre = configuracionSuaNivel.ConfSuaNNombre,
            //        ConfiguracionSua = configuracionSuaNivel.ConfiguracionSua,
            //        SuaExcel = configuracionSuaNivel.SuaExcel.ToList(),
            //        ConfiguracionSuaId = configuracionSuaNivel.ConfiguracionSuaId
            //    });
            //}
            //}
            //return Ok(responses);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfiguracionSua(int id, ConfiguracionSua model)
        {
            //if (id != configuracionSua.ConfiguracionSuaId)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(configuracionSua).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ConfiguracionSuaExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();

            if (id != model.ConfiguracionSuaId)
            {
                return BadRequest();
            }

            _context.Entry(model).State = EntityState.Modified;

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

            Respuesta respuesta = new Respuesta();
            //return NoContent();



            try
            {
                using (DataContext db = _context)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Se eliminan los registros existentes
                            var modelConfiguracionSuaNiveles = await _context.ConfiguracionSuaNiveles.Where(d => d.ConfiguracionSuaId == model.ConfiguracionSuaId).ToListAsync();

                            foreach (var modelConfiguracionSuaNivel in modelConfiguracionSuaNiveles)
                            {
                                var suaExceles = await _context.SuaExcels.Where(d => d.ConfiguracionSuaNivelId == modelConfiguracionSuaNivel.ConfiguracionSuaNivelId).ToListAsync();

                                foreach (var suaExcel in suaExceles)
                                {
                                    _context.SuaExcels.Remove(suaExcel);
                                    await _context.SaveChangesAsync();
                                }
                                _context.ConfiguracionSuaNiveles.Remove(modelConfiguracionSuaNivel);
                                await _context.SaveChangesAsync();
                            }
                            //transaction.Commit();

                            var configuracionSua = new ConfiguracionSua();
                            //configuracionSua.ConfSuaNombre = model.ConfSuaNombre;
                            //configuracionSua.ConfSuaEstatus = model.ConfSuaEstatus;
                            //db.ConfiguracionSuas.Add(configuracionSua);
                            //db.SaveChanges();

                            foreach (var modelConfiguracionSuaNivel in model.ConfiguracionSuaNivel)
                            {
                                var configuracionSuaNivel = new ConfiguracionSuaNivel();
                                configuracionSuaNivel.ConfiguracionSuaId = id;
                                configuracionSuaNivel.ConfSuaNNombre = modelConfiguracionSuaNivel.ConfSuaNNombre;
                                db.ConfiguracionSuaNiveles.Add(configuracionSuaNivel);
                                db.SaveChanges();

                                foreach (var modelSuaExcel in modelConfiguracionSuaNivel.SuaExcel)
                                {
                                    var suaExcel = new SuaExcel();
                                    suaExcel.ConfiguracionSuaNivelId =              configuracionSuaNivel.ConfiguracionSuaNivelId;
                                    suaExcel.TipoPeriodoId = modelSuaExcel.TipoPeriodoId;
                                    suaExcel.ExcelColumnaId = modelSuaExcel.ExcelColumnaId;
                                    db.SuaExcels.Add(suaExcel);
                                    db.SaveChanges();
                                }
                            }
                            transaction.Commit();
                            respuesta.Exito = 1;
                        }
                        catch (Exception)
                        {

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

        // POST: api/ConfiguracionSuas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ConfiguracionSua>> PostConfiguracionSua(ConfiguracionSua model)
        {
            var id = model.ConfiguracionSuaId;
            //_context.ConfiguracionSuas.Add(configuracionSua);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetConfiguracionSua", new { id = configuracionSua.ConfiguracionSuaId }, configuracionSua);

            //if (id != configuracionSua.ConfiguracionSuaId)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(configuracionSua).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ConfiguracionSuaExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();

            if (id != model.ConfiguracionSuaId)
            {
                return BadRequest();
            }

            _context.Entry(model).State = EntityState.Modified;

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

            Respuesta respuesta = new Respuesta();

            var respuestaE = await EliminarSuaNAsync(model);

            //var respuestaA = ActualizarSuaN(model);

            if (respuestaE.Exito == 1 )
            {
                respuesta.Exito = 1;
                respuesta.Mensaje = "Funciona bien";
            }
            else
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "No funciona bien";
            }

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
            Respuesta respuesta = new Respuesta();
            try
            {
                using (DataContext db = _context)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Se eliminan los registros existentes
                            var modelConfiguracionSuaNiveles = await _context.ConfiguracionSuaNiveles.Where(d => d.ConfiguracionSuaId == model.ConfiguracionSuaId).ToListAsync();

                            foreach (var modelConfiguracionSuaNivel in modelConfiguracionSuaNiveles)
                            {
                                var suaExceles = await _context.SuaExcels.Where(d => d.ConfiguracionSuaNivelId == modelConfiguracionSuaNivel.ConfiguracionSuaNivelId).ToListAsync();

                                foreach (var suaExcel in suaExceles)
                                {
                                    _context.SuaExcels.Remove(suaExcel);
                                    await _context.SaveChangesAsync();
                                }
                                _context.ConfiguracionSuaNiveles.Remove(modelConfiguracionSuaNivel);
                                await _context.SaveChangesAsync();
                            }

                            transaction.Commit();
                            respuesta.Exito = 1;
                        }
                        catch (Exception)
                        {

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
            return respuesta;
        }

        private Respuesta ActualizarSuaN(ConfiguracionSua model)
        {
            var id = model.ConfiguracionSuaId;
            Respuesta respuesta = new Respuesta();
            try
            {
                using (DataContext db = _context)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var configuracionSua = new ConfiguracionSua();
                            //configuracionSua.ConfSuaNombre = model.ConfSuaNombre;
                            //configuracionSua.ConfSuaEstatus = model.ConfSuaEstatus;
                            //db.ConfiguracionSuas.Add(configuracionSua);
                            //db.SaveChanges();

                            foreach (var modelConfiguracionSuaNivel in model.ConfiguracionSuaNivel)
                            {
                                var configuracionSuaNivel = new ConfiguracionSuaNivel();
                                configuracionSuaNivel.ConfiguracionSuaId = id;
                                configuracionSuaNivel.ConfSuaNNombre = modelConfiguracionSuaNivel.ConfSuaNNombre;
                                db.ConfiguracionSuaNiveles.Add(configuracionSuaNivel);
                                db.SaveChanges();

                                foreach (var modelSuaExcel in modelConfiguracionSuaNivel.SuaExcel)
                                {
                                    var suaExcel = new SuaExcel();
                                    suaExcel.ConfiguracionSuaNivelId = configuracionSuaNivel.ConfiguracionSuaNivelId;
                                    suaExcel.TipoPeriodoId = modelSuaExcel.TipoPeriodoId;
                                    suaExcel.ExcelColumnaId = modelSuaExcel.ExcelColumnaId;
                                    db.SuaExcels.Add(suaExcel);
                                    db.SaveChanges();
                                }
                            }
                            transaction.Commit();
                            respuesta.Exito = 1;
                        }
                        catch (Exception)
                        {

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
            return respuesta;
        }
    }
}
