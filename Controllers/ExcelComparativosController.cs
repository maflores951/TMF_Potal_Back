using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using LoginBase.Models.Excel;
using LoginBase.Models.Response;
using Microsoft.AspNetCore.Authorization;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExcelComparativosController : ControllerBase
    {
        private readonly DataContext _context;

        public ExcelComparativosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ExcelComparativos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExcelComparativo>>> GetExcelComparativo()
        {
            return await _context.ExcelComparativos.ToListAsync();
        }

        [HttpPost("RecuperaExcelCom")]
        public async Task<ActionResult<ExcelComparativo>> RecuperaExcelCom(ReporteExcelCom model)
        {
            //Se crea la respuesta a retornar
            Respuesta respuesta = new Respuesta();

            var configuracionSua = await _context.ConfiguracionSuas.
              Where(u => u.ConfiguracionSuaId == model.ConfiguracionSuaId).FirstOrDefaultAsync();

            //Se recuperan los excel registrados
            var excelComparativo = await _context.ExcelComparativos.
               Where(u => u.excelComparativoMes == model.EmpleadoColumnaMes && u.excelComparativoAnio == model.EmpleadoColumnaAnio && u.UsuarioId == model.UsuarioId && u.ExcelTipoPeriodo == configuracionSua.ConfiguracionSuaTipo).
               ToListAsync();



            if (excelComparativo.Count == 0)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "No existen documentos registrados.";
                return Ok(respuesta);
            }

            var archivos = "";

            //if (configuracionSua.ConfiguracionSuaTipo == 2)
            //{
            //    var excelSua = await _context.ExcelComparativos.
            //                  Where(u => u.excelComparativoMes == model.EmpleadoColumnaMes && u.excelComparativoAnio == model.EmpleadoColumnaAnio && u.UsuarioId == model.UsuarioId && u.excelTipoId == 4).FirstOrDefaultAsync();
            //    if (excelSua != null)
            //    {
            //        archivos += excelSua.excelComparativoNombre + Environment.NewLine;
            //    }
            //}
            

            
            foreach (var item in excelComparativo)
            {
                archivos += item.excelComparativoNombre + Environment.NewLine;

                //item.excelComparativoNombre
            }

            respuesta.Exito = 1;
            respuesta.Mensaje = "Se compararán los siguientes documentos:" + Environment.NewLine + archivos;
            respuesta.Data = excelComparativo;
            return Ok(respuesta);
        }

        // GET: api/ExcelComparativos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExcelComparativo>> GetExcelComparativo(int id)
        {
            var excelComparativo = await _context.ExcelComparativos.FindAsync(id);

            if (excelComparativo == null)
            {
                return NotFound();
            }

            return excelComparativo;
        }

        // PUT: api/ExcelComparativos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExcelComparativo(int id, ExcelComparativo excelComparativo)
        {
            if (id != excelComparativo.excelComparativoId)
            {
                return BadRequest();
            }

            _context.Entry(excelComparativo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExcelComparativoExists(id))
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

        // POST: api/ExcelComparativos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ExcelComparativo>> PostExcelComparativo(ExcelComparativo excelComparativo)
        {
            _context.ExcelComparativos.Add(excelComparativo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExcelComparativo", new { id = excelComparativo.excelComparativoId }, excelComparativo);
        }

        //// DELETE: api/ExcelComparativos/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ExcelComparativo>> DeleteExcelComparativo(int id)
        //{
        //    var excelComparativo = await _context.ExcelComparativo.FindAsync(id);
        //    if (excelComparativo == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ExcelComparativo.Remove(excelComparativo);
        //    await _context.SaveChangesAsync();

        //    return excelComparativo;
        //}

        private bool ExcelComparativoExists(int id)
        {
            return _context.ExcelComparativos.Any(e => e.excelComparativoId == id);
        }
    }
}
