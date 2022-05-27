using System;
using System.Linq;
using System.Threading.Tasks;
using LoginBase.Models;
using LoginBase.Models.Request;
using LoginBase.Models.Response;
using LoginBase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]

    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private IUserService _userService;
        private readonly IWebHostEnvironment _enviroment;

        //Constructor para recuperar la interface del userService y el contexto de la base de datos
        public UserController(IUserService userService, DataContext db, IWebHostEnvironment env)
        {
            _userService = userService;
            _context = db;
            _enviroment = env;
        }

        
        //Login del usuario y creación de token
        [HttpPost("login")]
        public IActionResult Autentificar([FromBody] AuthRequest model)
        {
            //Se crea la respuesta
            Respuesta respuesta = new Respuesta();

            //Se crea la variable para asignar el empleado 
            Usuario usuario = new Usuario();

            //Se valida el login y se crea el token
            var userResponse = _userService.Auth(model);

            //Si no existe se asigna un mensaje de que el usuario es incorrecto y se retorna la respuesta
            if (userResponse == null)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "Usuario o contraseña incorrecta";
                return Ok(respuesta);
            }

            if (userResponse.UsuarioFechaLimite < DateTime.Now)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "La contraseña expiro, por favor recupere una nueva.";
                return Ok(respuesta);
            }
            //Si el usuario existe se retorna el usuario.
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
            Respuesta respuesta = new Respuesta();
            //dynamic jsonObject = form;

            try
            {
                email = model.Email;
            }
            catch
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "El email es incorrecto, favor de verificar.";
                return Ok(respuesta);
                //return BadRequest("Incorrect call");
            }


            //Se busca la información del usuario en la tabla Users por medio del email
            var usuario =  await _context.Usuarios.
               Where(u => u.Email.ToLower() == email.ToLower()).
               FirstOrDefaultAsync();

            //Se valida si existe el usuario
            if (usuario == null)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "El email no está registrado, favor de verificar";
                return Ok(respuesta);
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
                    respuesta.Exito = 0;
                    respuesta.Mensaje = "El email no está registrado, favor de verificar";
                    return Ok(respuesta);
                }
                else
                {
                    throw;
                }
            }

            //Se envia el email si todo es correcto
            EnvioEmailService enviarEmail = new EnvioEmailService(_context,_enviroment);
            var emailResponse = await enviarEmail.EnivarEmail(usuarioEmail);


           
            if (emailResponse.Exito == 1) {
                respuesta.Exito = 1;
                respuesta.Data = emailResponse;
            }
            else
            {
                respuesta.Exito = 0;
                respuesta.Data = emailResponse;
                respuesta.Mensaje = "Error en el servidor, contacte al administrador del sistema.";
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
