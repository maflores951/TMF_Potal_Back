using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoginBase.Models;
using LoginBase.Models.Response;
using LoginBase.Models.Sua;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuaController : ControllerBase
    {
        private readonly DataContext _context;

        public SuaController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Add(ConfiguracionSua model)
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
                            var configuracionSua = new ConfiguracionSua();
                            configuracionSua.ConfSuaNombre = model.ConfSuaNombre;
                            configuracionSua.ConfSuaEstatus = model.ConfSuaEstatus;
                            db.ConfiguracionSuas.Add(configuracionSua);
                            db.SaveChanges();

                            foreach (var modelConfiguracionSuaNivel in model.ConfiguracionSuaNivel)
                            {
                                var configuracionSuaNivel = new ConfiguracionSuaNivel();
                                configuracionSuaNivel.ConfiguracionSuaId = configuracionSua.ConfiguracionSuaId;
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

                respuesta.Mensaje = ex.Message ;
                respuesta.Exito = 0;
            }

            return Ok(respuesta);
        }
    }
}
