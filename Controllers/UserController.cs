using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LoginBase.Models;
using LoginBase.Models.Request;
using LoginBase.Models.Response;
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
    
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private IUserService _userService;

        public UserController(IUserService userService, DataContext db)
        {
            _userService = userService;
            _context = db;
        }

        [HttpPost("excel")]
        public IActionResult ExportarExcel(AuthRequest model)
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //string excelContentType = "application/vnd.ms-excel";
            var productos = _context.EmpleadoColumnas.AsNoTracking().ToList();
            FileContentResult excel ;
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

                    hoja.Cells["B" + col].Formula = "=SUBSTITUTE(ADDRESS(1,"+ col + ",4),\"1\",\"\")"; 
                    //hoja.Cells["B" + col].Style.Numberformat.Format = "dd/mm/aaaa";
                    //hoja.Cells["B" + col].Formula = "=SUMA(2+2)";
                }




                //Agregar formato de tabla
                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: productos.Count + 1, toColumn: 5), "Productos");
                tabla.ShowHeader = true;
                tabla.TableStyle = TableStyles.Light6;
                tabla.ShowTotal = true;


                excel = File(libro.GetAsByteArray(), excelContentType, "Productos.xlsx");
            }

            imagebase64 = Convert.ToBase64String(excel.FileContents);
            Respuesta respuesta = new Respuesta();

            respuesta.Data = imagebase64;

            return Ok(respuesta);
        }
        //Login del usuario y creación de token
        [HttpPost("login")]
        public IActionResult Autentificar([FromBody] AuthRequest model)
        {
            Respuesta respuesta = new Respuesta();

            Usuario usuario = new Usuario();

            var userResponse = _userService.Auth(model);

            if (userResponse == null)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "Usuario o contraseña incorrecta";
                return Ok(respuesta);
            }

            respuesta.Exito = 1;
            respuesta.Data = userResponse;


            return Ok(respuesta);
        }


        // POST: api/Users/EnviarEmail
        [HttpPost]
        [Route("EnviarEmail")]
        public async Task<IActionResult> EnviarEmailAsync([FromBody] Usuario model)
        {
            var email = string.Empty;
            RecuperaPassParametro usuarioEmail;
            //dynamic jsonObject = form;

            try
            {
                email = model.Email;
            }
            catch
            {
                return BadRequest("Incorrect call");
            }


            //Se busca la información del usuario en la tabla Users por medio del email
            var usuario =  await _context.Usuarios.
               Where(u => u.Email.ToLower() == email.ToLower()).
               FirstOrDefaultAsync();

            //Se valida si existe el usuario
            if (usuario == null)
            {
                return NotFound();
            }


            //Se prepara el email (Aqui se debe de asignar el email)
            var random = new Random();
            var numConfirmacion = random.Next(100000, 999999);


            //Se actualizan los datos del Usuario asignando el codigo de seguridad
            usuario.UsuarioEstatusSesion = false;
            usuario.UsuarioFechaLimite = DateTime.Now.AddMinutes(10);
            usuario.UsuarioNumConfirmacion = numConfirmacion;
            var id = usuario.UsuarioId;

            usuarioEmail = new RecuperaPassParametro
            {
                Email = email,
                UsuarioFechaLimite = DateTime.Now.AddMinutes(10),
                UsuarioId = id
            };

            //Se prepara el registro para que sea actualizado
            _context.Entry(usuario).State = EntityState.Modified;

            //Se valida que exista el usuario y si es asi lo actualiza
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //Se envia el email si todo es correcto
            EnvioEmailService enviarEmail = new EnvioEmailService(_context);
            var emailResponse = await enviarEmail.EnivarEmail(usuarioEmail);


            Respuesta respuesta = new Respuesta();
            if (emailResponse.Exito == 1) {
                respuesta.Exito = 1;
                respuesta.Data = emailResponse;
            }
            else
            {
                respuesta.Exito = 0;
                respuesta.Data = emailResponse;
            }
            
            //return (IActionResult)emailResponse;
            return Ok(respuesta);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
