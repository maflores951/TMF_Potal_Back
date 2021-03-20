using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using LoginBase.Models;
using LoginBase.Models.Empleado;
using LoginBase.Models.Excel;
using LoginBase.Models.Response;
using LoginBase.Models.Sua;
using LoginBase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SuaController : ControllerBase
    {
        private readonly DataContext _context;
        private IComparativoEspecial _comparativoEspecial;

        public SuaController(DataContext context, IComparativoEspecial comparativoEspecial)
        {
            _context = context;
            _comparativoEspecial = comparativoEspecial;
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
                                    suaExcel.ExcelTipoId = modelSuaExcel.ExcelTipoId;
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

        [HttpPost("Excel")]
        public async Task<IActionResult> ExportarExcelAsync(ReporteExcelCom model)
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //string excelContentType = "application/vnd.ms-excel";
            //var empleadoColumnas  = _context.EmpleadoColumnas.AsNoTracking().ToList();

            var excelTipos = await _context.ExcelTipos.ToListAsync();

            var configuracionSuaNiveles = await _context.ConfiguracionSuaNiveles.
               Where(u => u.ConfiguracionSuaId == model.ConfiguracionSuaId).
               ToListAsync();




            //var empleadoColumnas = await _context.EmpleadoColumnas.
            //   Where(u => u.ConfiguracionSuaId == model.ConfiguracionSuaId && u.EmpleadoColumnaAnio == model.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == model.EmpleadoColumnaMes).
            //   ToListAsync();

            var empleadoColumnas = await _context.EmpleadoColumnas.
              Where(u => u.EmpleadoColumnaAnio == model.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == model.EmpleadoColumnaMes && u.UsuarioId == model.UsuarioId).
              ToListAsync();

            var queryEmpleadoC =
       from empleadoColumna in empleadoColumnas
       group empleadoColumna by empleadoColumna.EmpleadoColumnaNo into newGroup
       orderby newGroup.Key
       select newGroup;

            FileContentResult excel;
            String imagebase64;
            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("Comparativo");
                //worksheet.Cells["A1"].LoadFromCollection(productos, PrintHeaders: true);

                //ExcelWorksheet hoja = libro.Workbook.Worksheets.Add("MiHoja de Excel");

                worksheet.Cells[1, 1].Value = "Empleado";

                worksheet.Cells[1, 2, 1, 3].Merge = true;
                worksheet.Cells[1, 4, 1, 5].Merge = true;
                worksheet.Cells[1, 6, 1, 7].Merge = true;

                worksheet.Cells[1, 2].Value = "Sistema de Nomina";

                worksheet.Cells[1, 4].Value = "Sistema IMSS";

                worksheet.Cells[1, 6].Value = "Emisión IMSS";
                var fila = 2;
                var col = 1;

                worksheet.Cells[fila, 1].Value = "Empleado";
                worksheet.Cells[fila, 2].Value = "Columna";
                worksheet.Cells[fila, 3].Value = "Dato";

                worksheet.Cells[fila, 4].Value = "Columna";
                worksheet.Cells[fila, 5].Value = "Dato";

                worksheet.Cells[fila, 6].Value = "Columna";
                worksheet.Cells[fila, 7].Value = "Dato";
                worksheet.Cells[fila, 8].Value = "Estatus";

                foreach (var empleadoColumna in queryEmpleadoC)
                {


                    foreach (var configuracionSuaNivel in configuracionSuaNiveles)
                    {
                        fila++;
                        string valorTemM = null;
                        string valorSua = null;
                        string valorEma = null;

                        decimal valorTemMInt = 0;
                        decimal valorSuaInt = 0;
                        decimal valorEmaInt = 0;
                        var result = false;

                        int excelPosicionTem = 0;
                        int excelPosicionSua = 0;
                        int excelPosicionEma = 0;

                        string empleadoValor = null;
                        int posicionExcel = 0;
                        string valor = "Hola";
                        EmpleadoColumna valorEmpleado = new EmpleadoColumna();
                        ExcelColumna excelColumnaCom = new ExcelColumna();

                        var suaExcels = await _context.SuaExcels.
                   Where(u => u.ConfiguracionSuaNivelId == configuracionSuaNivel.ConfiguracionSuaNivelId).
                   ToListAsync();

                        foreach (var suaExcel in suaExcels)
                        {

                            //Numero de empleado

                                //valorEmpleado = empleadoColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId && x.EmpleadoColumnaNo == empleadoColumna.Key).FirstOrDefault();
                            if (suaExcel.ExcelTipoId == 1)
                            {
                                excelColumnaCom = await _context.ExcelColumnas.Where(u => u.ExcelColumnaId == suaExcel.ExcelColumnaId).FirstOrDefaultAsync();
                            }

                            var valoresEmpleado = empleadoColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId && x.EmpleadoColumnaNo == empleadoColumna.Key).ToList();
                            //if (empleadoColumna.ExcelColumnaId == suaExcel.ExcelColumnaId)
                            //    {

                            if (valoresEmpleado != null)
                            {
                                foreach (var valorEmp in valoresEmpleado)
                                {
                                    valorEmpleado = valorEmp;
                                    suaExcel.ExcelColumna = _context.ExcelColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId).FirstOrDefault();

                                    //var columnas = _context.ExcelColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId).ToListAsync();

                                    int caseSwitch = suaExcel.ExcelColumna.ExcelTipoId;
                                    valor = valorEmpleado.EmpleadoColumnaValor.Trim();

                                    empleadoValor = valorEmpleado.EmpleadoColumnaNo.Trim();
                                    decimal valorInt = 0;

                                    if (caseSwitch == 5 || caseSwitch == 6)
                                    {
                                        if (configuracionSuaNivel.ConfSuaNNombre == "Días" || configuracionSuaNivel.ConfSuaNNombre == "Retiro" ||
                                            configuracionSuaNivel.ConfSuaNNombre == "Cesantía en Edad Avanzada y Vejez Patronal" || configuracionSuaNivel.ConfSuaNNombre == "Cesantía en Edad Avanzada y Vejez Obrero" || configuracionSuaNivel.ConfSuaNNombre == "Subtotal RCV" || configuracionSuaNivel.ConfSuaNNombre == "Aportación Patronal" || configuracionSuaNivel.ConfSuaNNombre == "Valor de Descuento" || configuracionSuaNivel.ConfSuaNNombre == "Amortización" || configuracionSuaNivel.ConfSuaNNombre == "Subtotal Infonavit" || configuracionSuaNivel.ConfSuaNNombre == "Total")
                                        {
                                            result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                            if (result)
                                            {
                                                valorEmaInt += valorInt;
                                            }
                                            else
                                            {
                                                valorEma += valor + " ";
                                            }
                                            //}
                                            //else
                                            //{

                                            //} //result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                            if (result)
                                            {
                                                valorEmaInt += valorInt;
                                            }
                                            else
                                            {
                                                valorEma += valor + " ";
                                            }
                                        }
                                        else
                                        {
                                            if(valor != "")
                                            {
                                                result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                                if (result)
                                                {
                                                    valorEmaInt = valorInt;
                                                }
                                                else
                                                {
                                                    valorEma = valor + " ";
                                                }
                                                excelPosicionEma = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                            }
                                            
                                        }
                                    }
                                    else
                                    {
                                        switch (caseSwitch)
                                        {
                                            case 2:
                                                result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                                if (result)
                                                {

                                                    valorTemMInt += valorInt;
                                                }
                                                else
                                                {
                                                    valorTemM += valor + " ";
                                                }

                                                excelPosicionTem = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                                break;
                                            case 3:
                                                result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                                if (result)
                                                {
                                                    valorTemMInt += valorInt;
                                                }
                                                else
                                                {
                                                    valorTemM += valor + " ";
                                                }

                                                excelPosicionTem = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                                break;
                                            case 4:
                                                result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                                if (result)
                                                {
                                                    valorSuaInt += valorInt;
                                                }
                                                else
                                                {
                                                    valorSua += valor + " ";
                                                }



                                                excelPosicionSua = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                                break;
                                            //case 5:
                                            //    result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                            //    if (result)
                                            //    {
                                            //        valorEmaInt += valorInt;
                                            //    }
                                            //    else
                                            //    {
                                            //        valorEma += valor + " ";
                                            //    }


                                            //    excelPosicionEma = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                            //    break;
                                            //case 6:
                                            //    result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                            //    if (result)
                                            //    {
                                            //        valorEmaInt += valorInt;
                                            //    }
                                            //    else
                                            //    {
                                            //        valorEma += valor + " ";
                                            //    }
                                            //    excelPosicionEma = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                            //    break;

                                            default:
                                                Console.WriteLine("Default case");
                                                break;
                                        }
                                    }



                                }
                            }
                           
                        }

                        //}
                        if (excelPosicionTem > 0)
                        {
                            if (result)
                            {
                                valorTemM = valorTemMInt.ToString().Trim();
                            }
                            else
                            {
                                if (valorTemM.IndexOf("$") > 0)
                                {
                                    valorTemM = valorTemM.Replace("$", " ").Trim();
                                }

                            }
                        }

                        if (excelPosicionSua > 0)
                        {
                            if (result)
                            {
                                valorSua = valorSuaInt.ToString();
                            }
                            else
                            {
                                if (valorSua.IndexOf("$") > 0)
                                {
                                    valorSua = valorSua.Replace("$", " ").Trim();
                                }
                            }
                        }

                        if (excelPosicionEma > 0)
                        {
                            if (result)
                            {
                                valorEma = valorEmaInt.ToString();
                            }
                            else
                            {
                                if (valorEma.IndexOf("$") > 0)
                                {
                                    valorEma = valorEma.Replace("$", " ").Trim();
                                }
                            }
                        }



                        //var prueba = String.Equals(valorTemM.Trim(), valorSua.Trim());
                        //var pruebas = String.Equals(valorTemM.Trim(), valorEma.Trim());
                        //var pruebae = String.Equals(valorSua.Trim(), valorEma.Trim());
                        var estatusComparacion = false;
                        if (valorTemM != null && valorSua != null)
                        {
                            if (String.Equals(valorTemM.Trim(), valorSua.Trim()))
                            {
                                if (valorEma != null)
                                {
                                    if (String.Equals(valorTemM.Trim(), valorEma.Trim()))
                                    {
                                        estatusComparacion = true;
                                    }
                                    else
                                    {
                                        estatusComparacion = false;
                                    }
                                }
                                else
                                {
                                    estatusComparacion = true;
                                }
                            }
                            else
                            {
                                estatusComparacion = false;
                            }
                        }
                        else
                        {
                            if (valorTemM != null && valorEma != null)
                            {
                                if (String.Equals(valorTemM.Trim(), valorEma.Trim()))
                                {
                                    estatusComparacion = true;
                                }
                                else
                                {
                                    estatusComparacion = false;
                                }
                            }
                            else
                            {
                                if (valorSua != null && valorEma != null)
                                {
                                    if (String.Equals(valorSua.Trim(), valorEma.Trim()))
                                    {
                                        estatusComparacion = true;
                                    }
                                    else
                                    {
                                        estatusComparacion = false;
                                    }
                                }
                                else
                                {
                                    estatusComparacion = false;
                                }
                            }
                        }

                        if (excelColumnaCom.ExcelColumnaNombre == "Comparativo +-0.05")
                        {
                            estatusComparacion = _comparativoEspecial.CompararCUOTAS_OP_RCV(Convert.ToDouble(valorTemM), Convert.ToDouble(valorSua), Convert.ToDouble(valorEma));
                            //estatusComparacion = _comparativoEspecial.CompararCUOTAS_OP_RCV(25.50, 25.50, 25.54);
                        }

                        if (excelColumnaCom.ExcelColumnaNombre == "Comparativo +-1")
                        {
                            estatusComparacion = _comparativoEspecial.CR_INFONAVIT(Convert.ToDouble(valorTemM), Convert.ToDouble(valorSua), Convert.ToDouble(valorEma));
                        }

                        //worksheet.Cells[fila, 1].Value = empleadoValor;

                        worksheet.Cells[fila, 1].Value = empleadoValor;

                        if (excelPosicionTem > 0)
                        {
                            worksheet.Cells[fila, 2].Formula = "=SUBSTITUTE(ADDRESS(1," + excelPosicionTem + ",4),\"1\",\"\")";
                        }
                        else
                        {
                            worksheet.Cells[fila, 2].Value = "NA";
                        }

                        worksheet.Cells[fila, 3].Value = valorTemM;

                        if (excelPosicionSua > 0)
                        {
                            worksheet.Cells[fila, 4].Formula = "=SUBSTITUTE(ADDRESS(1," + excelPosicionSua + ",4),\"1\",\"\")";
                        }
                        else
                        {
                            worksheet.Cells[fila, 4].Value = "NA";
                        }

                        worksheet.Cells[fila, 5].Value = valorSua;

                        if (excelPosicionEma > 0)
                        {
                            worksheet.Cells[fila, 6].Formula = "=SUBSTITUTE(ADDRESS(1," + excelPosicionEma + ",4),\"1\",\"\")";
                        }
                        else
                        {
                            worksheet.Cells[fila, 6].Value = "NA";
                        }

                        worksheet.Cells[fila, 7].Value = valorEma;
                        worksheet.Cells[fila, 8].Value = estatusComparacion.ToString();

                        //worksheet.Column(col).AutoFit();
                        //worksheet.Cells[1,col].Value = "=SUBSTITUTE(DIRECCION(1,1,4),\"1\",\"\")";
                        //worksheet.Cells[1, col].Style.Font.Color.SetColor(Color.Red);
                        //worksheet.Cells[1, col].Style.Font.Name = "Calibri";
                        //worksheet.Cells[1, col].Style.Font.Size = 40;

                        //worksheet.Cells[2,col].Formula = "=SUBSTITUTE(ADDRESS(1," + col + ",4),\"1\",\"\")";
                        //hoja.Cells["B" + col].Style.Numberformat.Format = "dd/mm/aaaa";
                        //hoja.Cells["B" + col].Formula = "=SUMA(2+2)";
                    }




                    //Agregar formato de tabla
                    //var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: productos.Count + 1, toColumn: 5), "Productos");
                    //tabla.ShowHeader = true;
                    //tabla.TableStyle = TableStyles.Light6;
                    //tabla.ShowTotal = true;

                }
                excel = File(libro.GetAsByteArray(), excelContentType, "Comparativo.xlsx");
            }

            imagebase64 = Convert.ToBase64String(excel.FileContents);
            Respuesta respuesta = new Respuesta();

            respuesta.Data = imagebase64;

            return Ok(respuesta);
        }
    }
}
