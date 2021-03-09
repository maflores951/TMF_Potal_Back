using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using LoginBase.Models;
using LoginBase.Models.Response;
using LoginBase.Models.Sua;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;

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
            var id = model.ConfiguracionSuaId;
            try
            {
                using (DataContext db = _context)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (id <= 0)
                            {
                                var configuracionSua = new ConfiguracionSua();
                                configuracionSua.ConfSuaNombre = model.ConfSuaNombre;
                                configuracionSua.ConfSuaEstatus = model.ConfSuaEstatus;
                                db.ConfiguracionSuas.Add(configuracionSua);
                                db.SaveChanges();
                                id = configuracionSua.ConfiguracionSuaId;
                            }
                           

                            foreach (var modelConfiguracionSuaNivel in model.ConfiguracionSuaNivel)
                            {
                                var configuracionSuaNivel = new ConfiguracionSuaNivel();
                                configuracionSuaNivel.ConfiguracionSuaId = id;//configuracionSua.ConfiguracionSuaId;
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

        [HttpGet("Excel")]
        public IActionResult ExportarExcel()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //string excelContentType = "application/vnd.ms-excel";
            var productos = _context.EmpleadoColumnas.AsNoTracking().ToList();
            FileContentResult excel;
            String imagebase64;
            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("Productos");
                worksheet.Cells["A1"].LoadFromCollection(productos, PrintHeaders: true);

                ExcelWorksheet hoja = libro.Workbook.Worksheets.Add("MiHoja de Excel");
                for (var col = 1; col < productos.Count + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                    hoja.Cells["A" + col].Value = "=SUBSTITUTE(DIRECCION(1,1,4),\"1\",\"\")";
                    hoja.Cells["A" + col].Style.Font.Color.SetColor(Color.Red);
                    hoja.Cells["A" + col].Style.Font.Name = "Calibri";
                    hoja.Cells["A" + col].Style.Font.Size = 40;

                    hoja.Cells["B" + col].Formula = "=SUBSTITUTE(ADDRESS(1," + col + ",4),\"1\",\"\")";
                    //hoja.Cells["B" + col].Style.Numberformat.Format = "dd/mm/aaaa";
                    //hoja.Cells["B" + col].Formula = "=SUMA(2+2)";
                }




                //Agregar formato de tabla
                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: productos.Count + 1, toColumn: 5), "Productos");
                tabla.ShowHeader = true;
                tabla.TableStyle = TableStyles.Light6;
                tabla.ShowTotal = true;


                excel = File(libro.GetAsByteArray(), excelContentType, "Comparativo.xlsx");
            }

            imagebase64 = Convert.ToBase64String(excel.FileContents);
            Respuesta respuesta = new Respuesta();

            respuesta.Data = imagebase64;

            return Ok(respuesta);
        }
    }
}
