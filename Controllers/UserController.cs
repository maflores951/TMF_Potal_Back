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
    //[Authorize]

    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private IUserService _userService;

        public UserController(IUserService userService, DataContext db)
        {
            _userService = userService;
            _context = db;
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
        [Authorize]
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
