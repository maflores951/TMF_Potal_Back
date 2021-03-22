using System;
using System.Linq;
using System.Threading.Tasks;
using LoginBase.Models;
using LoginBase.Models.Empleado;
using LoginBase.Models.Excel;
using LoginBase.Models.Response;
using LoginBase.Models.Sua;
using LoginBase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SuaController : ControllerBase
    {
        private readonly DataContext _context;
        private IComparativoEspecial _comparativoEspecial;

        //Constructor para recuperar el contexto e impementar la interface para hacer el comparativo especial
        public SuaController(DataContext context, IComparativoEspecial comparativoEspecial)
        {
            _context = context;
            _comparativoEspecial = comparativoEspecial;
        }

        //Se utiliza para guardar la inforamción de cada configuración
        [HttpPost]
        public IActionResult Add(ConfiguracionSua model)
        {
            //Se crea la respuesta a retornar
            Respuesta respuesta = new Respuesta();

            //Se recupera el id
            var id = model.ConfiguracionSuaId;
            try
            {
                //Se recupera el contextexto para conectar con la base de datos
                using (DataContext db = _context)
                {
                    //Se crea una trasaction para cuidar la base de datos
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Si no existe la configuración se crea una nueva
                            if (id <= 0)
                            {
                                var configuracionSua = new ConfiguracionSua();
                                configuracionSua.ConfSuaNombre = model.ConfSuaNombre;
                                configuracionSua.ConfSuaEstatus = model.ConfSuaEstatus;
                                db.ConfiguracionSuas.Add(configuracionSua);
                                db.SaveChanges();
                                id = configuracionSua.ConfiguracionSuaId;
                            }

                            //Se recorren los niveles
                            foreach (var modelConfiguracionSuaNivel in model.ConfiguracionSuaNivel)
                            {
                                //Se agregan los nivels con respecto a la configuración
                                var configuracionSuaNivel = new ConfiguracionSuaNivel();
                                configuracionSuaNivel.ConfiguracionSuaId = id;//configuracionSua.ConfiguracionSuaId;
                                configuracionSuaNivel.ConfSuaNNombre = modelConfiguracionSuaNivel.ConfSuaNNombre;
                                db.ConfiguracionSuaNiveles.Add(configuracionSuaNivel);
                                db.SaveChanges();

                                //Se recorren las filas de cada nivel y  se agregan a la base de datos
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
                            //Si algo sale mal se restaura la base de datos y no se guarda información erronea
                            transaction.Rollback();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                //Si algo sale mal no se ejecuta el proceso y recupera un error
                respuesta.Mensaje = ex.Message;
                respuesta.Exito = 0;
            }

            //Se recupera la respuesta
            return Ok(respuesta);
        }

        //Api para poder generar el excel de comparación
        [HttpPost("Excel")]
        public async Task<IActionResult> ExportarExcelAsync(ReporteExcelCom model)
        {
            //Se determina el formato de excel a utilizar
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //string excelContentType = "application/vnd.ms-excel";
            //var empleadoColumnas  = _context.EmpleadoColumnas.AsNoTracking().ToList();

            //Se recuperan los tipos de excel que existen
            var excelTipos = await _context.ExcelTipos.ToListAsync();

            //Se recuperan los niveles de la configuración
            var configuracionSuaNiveles = await _context.ConfiguracionSuaNiveles.
               Where(u => u.ConfiguracionSuaId == model.ConfiguracionSuaId).
               ToListAsync();




            //var empleadoColumnas = await _context.EmpleadoColumnas.
            //   Where(u => u.ConfiguracionSuaId == model.ConfiguracionSuaId && u.EmpleadoColumnaAnio == model.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == model.EmpleadoColumnaMes).
            //   ToListAsync();

            //Se recupera la información relacionada con respecto a los parametros
            var empleadoColumnas = await _context.EmpleadoColumnas.
              Where(u => u.EmpleadoColumnaAnio == model.EmpleadoColumnaAnio && u.EmpleadoColumnaMes == model.EmpleadoColumnaMes && u.UsuarioId == model.UsuarioId).
              ToListAsync();

            //Se ordenan los empleados para la exportación
            var queryEmpleadoC =
       from empleadoColumna in empleadoColumnas
       group empleadoColumna by empleadoColumna.EmpleadoColumnaNo into newGroup
       orderby newGroup.Key
       select newGroup;

            //Se crea la variable para almacenar el excel.
            FileContentResult excel;

            //Se crea la variable que retorna el excel en base64 para poder ser enviado
            String imagebase64;

            //Se crea una variable con el Nuget ExcelPackagePlus
            using (var libro = new ExcelPackage())
            {
                //Se crea la pestaña
                var worksheet = libro.Workbook.Worksheets.Add("Comparativo");
                //worksheet.Cells["A1"].LoadFromCollection(productos, PrintHeaders: true);

                //ExcelWorksheet hoja = libro.Workbook.Worksheets.Add("MiHoja de Excel");

                //Se crean los titulos de las columnas
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


                //Se recorren los empleados
                foreach (var empleadoColumna in queryEmpleadoC)
                {
                    //Se reccorren los niveles
                    foreach (var configuracionSuaNivel in configuracionSuaNiveles)
                    {
                        //Se va agregando la fila a trabajar
                        fila++;
                        //Variables que almacenan los valores
                        string valorTemM = null;
                        string valorSua = null;
                        string valorEma = null;

                        //Variables que almacenan los resultados numericos
                        decimal valorTemMInt = 0;
                        decimal valorSuaInt = 0;
                        decimal valorEmaInt = 0;

                        //Variable para determinar si existe un resultado
                        var result = false;

                        //Variables para determinar la posición de cada valor 
                        int excelPosicionTem = 0;
                        int excelPosicionSua = 0;
                        int excelPosicionEma = 0;

                        //Variable para determinar el número de empleado
                        string empleadoValor = null;
                        
                        //Variable para acomodar cada valor por tipo de excel
                        string valor = "Hola";

                        //Variables para determinar la información del excel y de el empleado
                        EmpleadoColumna valorEmpleado = new EmpleadoColumna();
                        ExcelColumna excelColumnaCom = new ExcelColumna();

                        //Se recupera las filas comparativas por cada nivel
                        var suaExcels = await _context.SuaExcels.
                   Where(u => u.ConfiguracionSuaNivelId == configuracionSuaNivel.ConfiguracionSuaNivelId).
                   ToListAsync();

                        //Se recorre la configuración 
                        foreach (var suaExcel in suaExcels)
                        {

                            //Numero de empleado

                                //valorEmpleado = empleadoColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId && x.EmpleadoColumnaNo == empleadoColumna.Key).FirstOrDefault();

                                //Si es del tipo 1 signific que es una comparación especial 
                            if (suaExcel.ExcelTipoId == 1)
                            {
                                //Se recupera el nombre de la configuración especial
                                excelColumnaCom = await _context.ExcelColumnas.Where(u => u.ExcelColumnaId == suaExcel.ExcelColumnaId).FirstOrDefaultAsync();
                            }

                            //Se recuperan las filas registradas por número de empleado y fila de excel
                            var valoresEmpleado = empleadoColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId && x.EmpleadoColumnaNo == empleadoColumna.Key).ToList();
                            //if (empleadoColumna.ExcelColumnaId == suaExcel.ExcelColumnaId)
                            //    {

                            //Si existe el valor comiensa a ordenarse la información
                            if (valoresEmpleado != null)
                            {
                                //Se recorren las filas encontradas
                                foreach (var valorEmp in valoresEmpleado)
                                {
                                    //Asigna cada fila 
                                    valorEmpleado = valorEmp;

                                    //Se recuperan los datos de la columna 
                                    suaExcel.ExcelColumna = _context.ExcelColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId).FirstOrDefault();

                                    //var columnas = _context.ExcelColumnas.Where(x => suaExcel.ExcelColumnaId == x.ExcelColumnaId).ToListAsync();

                                    //Se recupera el tipo de archivo
                                    int caseSwitch = suaExcel.ExcelColumna.ExcelTipoId;

                                    //Se recupera el valor de la casilla sin espacios
                                    valor = valorEmpleado.EmpleadoColumnaValor.Trim();
                                    //Se recupera el número de empleado
                                    empleadoValor = valorEmpleado.EmpleadoColumnaNo.Trim();

                                    //Se crea el valor numerico para comparar de forma especial
                                    decimal valorInt = 0;

                                    //Para los archivos en donde hay multiples columnas se valida que la fila sea sumable para no agrupar información erronea
                                    if (caseSwitch == 5 || caseSwitch == 6)
                                    {
                                        //Estas columas si se pueden agrupar
                                        if (configuracionSuaNivel.ConfSuaNNombre == "Días" || configuracionSuaNivel.ConfSuaNNombre == "Retiro" ||
                                            configuracionSuaNivel.ConfSuaNNombre == "Cesantía en Edad Avanzada y Vejez Patronal" || configuracionSuaNivel.ConfSuaNNombre == "Cesantía en Edad Avanzada y Vejez Obrero" || configuracionSuaNivel.ConfSuaNNombre == "Subtotal RCV" || configuracionSuaNivel.ConfSuaNNombre == "Aportación Patronal" || configuracionSuaNivel.ConfSuaNNombre == "Valor de Descuento" || configuracionSuaNivel.ConfSuaNNombre == "Amortización" || configuracionSuaNivel.ConfSuaNNombre == "Subtotal Infonavit" || configuracionSuaNivel.ConfSuaNNombre == "Total")
                                        {
                                            //Se intenta convertir en numerico 
                                            result = decimal.TryParse(valor, out valorInt); //i now = 108  

                                            //Si es numerico se guarda 
                                            if (result)
                                            {
                                                valorEmaInt += valorInt;
                                            }
                                            //Si no es numerico se guarda como string
                                            else
                                            {
                                                valorEma += valor + " ";
                                            }

                                            //if (result)
                                            //{
                                            //    valorEmaInt += valorInt;
                                            //}
                                            //else
                                            //{
                                            //    valorEma += valor + " ";
                                            //}
                                            excelPosicionEma = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                        }
                                        else
                                        {
                                            //Para las columnas que son unicas se valida que exista un valor
                                            if(valor != "")
                                            {
                                                //Se valida si es numerico 
                                                result = decimal.TryParse(valor, out valorInt); //i now = 108  
                                                //Si es numerico se guarda
                                                if (result)
                                                {
                                                    valorEmaInt = valorInt;
                                                }
                                                //Si no es numerico se guarda como string
                                                else
                                                {
                                                    valorEma = valor + " ";
                                                }
                                                //Se guarda la posición del excel
                                                excelPosicionEma = suaExcel.ExcelColumna.ExcelPosicion + 1;
                                            }
                                            
                                        }
                                    }
                                    else
                                    {
                                        //Para los archivos donde no hay columnas dobles se valida el tipo de archivo
                                        switch (caseSwitch)
                                        {
                                            //Se mete al case segun el tipo de archivo para poder asignar cada una de las variables y posteriormente compararlos, se determina si es numerico o string para poder hacer la agrupación
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
                        //Se determina si el dato existe segun su posición, si es numerico se convierte a string y se quitan los espaciós, si este es una suma de caracteres se cambia & por espacios.
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
                        //Se determina si el dato existe segun su posición, si es numerico se convierte a string y se quitan los espaciós, si este es una suma de caracteres se cambia & por espacios.
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
                        //Se determina si el dato existe segun su posición, si es numerico se convierte a string y se quitan los espaciós, si este es una suma de caracteres se cambia & por espacios.
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

                        //Se comiensa a evaluar si los datos son iguales, en caso de que sean vacios, el comparativo solo se hace con los valores existentes
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

                        //Coparativo especial para comparar con una diferencia de +-0.05
                        if (excelColumnaCom.ExcelColumnaNombre == "Comparativo +-0.05")
                        {
                            estatusComparacion = _comparativoEspecial.CompararCUOTAS_OP_RCV(Convert.ToDouble(valorTemM), Convert.ToDouble(valorSua), Convert.ToDouble(valorEma));
                            //estatusComparacion = _comparativoEspecial.CompararCUOTAS_OP_RCV(25.50, 25.50, 25.54);
                        }

                        //Coparativo especial para comparar con una diferencia de +-1
                        if (excelColumnaCom.ExcelColumnaNombre == "Comparativo +-1")
                        {
                            estatusComparacion = _comparativoEspecial.CR_INFONAVIT(Convert.ToDouble(valorTemM), Convert.ToDouble(valorSua), Convert.ToDouble(valorEma));
                        }

                        //worksheet.Cells[fila, 1].Value = empleadoValor;
                        //Se asigna el número de empleado
                        worksheet.Cells[fila, 1].Value = empleadoValor;

                        //Si existe una posición se agrega en una formula para determinar en letra la posición de la columna
                        if (excelPosicionTem > 0)
                        {
                            worksheet.Cells[fila, 2].Formula = "=SUBSTITUTE(ADDRESS(1," + excelPosicionTem + ",4),\"1\",\"\")";
                        }
                        //Si no existe asigna NA
                        else
                        {
                            worksheet.Cells[fila, 2].Value = "NA";
                        }

                        //Se asigna el valor del template mensual
                        worksheet.Cells[fila, 3].Value = valorTemM;

                        //Si existe una posición se agrega en una formula para determinar en letra la posición de la columna
                        if (excelPosicionSua > 0)
                        {
                            worksheet.Cells[fila, 4].Formula = "=SUBSTITUTE(ADDRESS(1," + excelPosicionSua + ",4),\"1\",\"\")";
                        }
                        else
                        {
                            worksheet.Cells[fila, 4].Value = "NA";
                        }

                        //Se asigna el valor del archivo SUA
                        worksheet.Cells[fila, 5].Value = valorSua;

                        //Si existe una posición se agrega en una formula para determinar en letra la posición de la columna
                        if (excelPosicionEma > 0)
                        {
                            worksheet.Cells[fila, 6].Formula = "=SUBSTITUTE(ADDRESS(1," + excelPosicionEma + ",4),\"1\",\"\")";
                        }
                        else
                        {
                            worksheet.Cells[fila, 6].Value = "NA";
                        }

                        //Se agrega el valor del archivo EMA
                        worksheet.Cells[fila, 7].Value = valorEma;

                        //Se asigna el estatus obtenido del comparativo
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
                //Se crea el archivo excel
                excel = File(libro.GetAsByteArray(), excelContentType, "Comparativo.xlsx");
            }

            //Se asigna el reporte como un string en base64
            imagebase64 = Convert.ToBase64String(excel.FileContents);

            //Se crea las respuesta
            Respuesta respuesta = new Respuesta();

            //Se asigna el base64 en la respuesta 
            respuesta.Data = imagebase64;

            //Se recupera la respuesta
            return Ok(respuesta);
        }
    }
}
