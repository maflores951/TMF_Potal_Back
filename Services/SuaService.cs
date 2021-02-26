using LoginBase.Models;
using LoginBase.Models.Response;
using LoginBase.Models.Sua;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Services
{
    public class SuaService
    {
        private readonly DataContext _context;

        public SuaService(DataContext db)
        {
            _context = db;
        }

        public async Task<Respuesta> EliminarSuaNAsync(ConfiguracionSua model)
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

        public Respuesta ActualizarSuaN(ConfiguracionSua model)
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
