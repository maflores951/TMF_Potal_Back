using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using LoginBase.Helper;
using LoginBase.Models.Response;
using Microsoft.AspNetCore.Hosting;
using Fintech.API.Helpers;

namespace LoginBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly IWebHostEnvironment _enviroment;
        public UsuariosController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _enviroment = env;
        }

        // GET: api/Usuarios
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            //return await _context.Usuarios.ToListAsync();
            var responses = new List<Usuario>();
            var usuarios = await _context.Usuarios.ToListAsync();
            //Se crea una variable del tipo de servicio para poder decifrar la contraseña
            CifradoHelper cifradoHelper = new CifradoHelper();

            foreach (var usuario in usuarios)
            {
                if(usuario.UsuarioEstatusSesion == false) { 
                var rol = await _context.Roles.FindAsync(usuario.RolId);
                var empresa = await _context.Empresas.FindAsync(usuario.EmpresaId);
                    var password = "";
                    if (usuario.Password == null) {
                         password = usuario.Password;
                    }
                    else{
                         password = cifradoHelper.DecryptStringAES(usuario.Password);
                    }
                    responses.Add(new Usuario
                {
                    UsuarioId = usuario.UsuarioId,
                    UsuarioNombre = usuario.UsuarioNombre,
                    UsuarioApellidoP = usuario.UsuarioApellidoP,
                    UsuarioApellidoM = usuario.UsuarioApellidoM,
                    UsuarioNumConfirmacion = usuario.UsuarioNumConfirmacion,
                    UsuarioFechaLimite = usuario.UsuarioFechaLimite,
                    UsuarioEstatusSesion = usuario.UsuarioEstatusSesion,
                    Password = password,
                    Email = usuario.Email,
                    UsuarioClave = usuario.UsuarioClave,
                    ImagePath = usuario.ImagePath,
                    RolId = usuario.RolId,
                    Rol = rol,
                    EmpleadoNoEmp = usuario.EmpleadoNoEmp,
                    EmpresaId = usuario.EmpresaId,
                    Empresa = empresa,
                    EmpleadoRFC = usuario.EmpleadoRFC
                    });
                }
            }

            return Ok(responses);
        }

        // GET: api/Usuarios
        [HttpGet]
        [Route("Empleados")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetEmpleados()
        {
            //return await _context.Usuarios.ToListAsync();
            var responses = new List<Usuario>();
            var usuarios = await _context.Usuarios.ToListAsync();
            //Se crea una variable del tipo de servicio para poder decifrar la contraseña
            CifradoHelper cifradoHelper = new CifradoHelper();

            foreach (var usuario in usuarios)
            {
                if (usuario.UsuarioEstatusSesion == false && usuario.EmpleadoNoEmp != null)
                {
                    var rol = await _context.Roles.FindAsync(usuario.RolId);
                    var empresa = await _context.Empresas.FindAsync(usuario.EmpresaId);
                    var password = "";
                    if (usuario.Password == null)
                    {
                        password = usuario.Password;
                    }
                    else
                    {
                        password = cifradoHelper.DecryptStringAES(usuario.Password);
                    }
                    responses.Add(new Usuario
                    {
                        UsuarioId = usuario.UsuarioId,
                        UsuarioNombre = usuario.UsuarioNombre,
                        UsuarioApellidoP = usuario.UsuarioApellidoP,
                        UsuarioApellidoM = usuario.UsuarioApellidoM,
                        UsuarioNumConfirmacion = usuario.UsuarioNumConfirmacion,
                        UsuarioFechaLimite = usuario.UsuarioFechaLimite,
                        UsuarioEstatusSesion = usuario.UsuarioEstatusSesion,
                        Password = password,
                        Email = usuario.Email,
                        UsuarioClave = usuario.UsuarioClave,
                        ImagePath = usuario.ImagePath,
                        RolId = usuario.RolId,
                        Rol = rol,
                        EmpleadoNoEmp = usuario.EmpleadoNoEmp,
                        EmpresaId = usuario.EmpresaId,
                        Empresa = empresa,
                        EmpleadoRFC = usuario.EmpleadoRFC
                    });
                }
            }

            return Ok(responses);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            //if (id != usuario.UsuarioId)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(usuario).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!UsuarioExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();

            //var usuarioBefore = await _context.Usuarios.FindAsync(id);

            //if (String.IsNullOrEmpty(usuario.Password))
            //{
            //    usuario.Password = usuarioBefore.Password;
            //}

            //var usuarioEmail = await _context.Usuarios.
            //  Where(u => u.Email.ToLower() == usuario.Email.ToLower()).
            //  FirstOrDefaultAsync();

            //if (usuarioEmail != null)
            //{
            //    return BadRequest("La cuenta de email que ingreso ya está registrada");
            //}

            if (usuario.ImageArray != null && usuario.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(usuario.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "uploads";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);
                //var response = false;
                if (response)
                {
                    usuario.ImagePath = fullPath;
                }
            }

            if (usuario.ImageBase64 != null && usuario.ImageBase64.Length > 0)
            {
                byte[] imageArray = Convert.FromBase64String(usuario.ImageBase64);
                var stream = new MemoryStream(imageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "uploads";//"~/Content/Images";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);

                if (response)
                {
                    usuario.ImagePath = fullPath;
                }
            }

            DateTime today = DateTime.Today;
            var diasPass = await _context.Parametros.
             Where(u => u.ParametroClave.ToLower() == "SETEXP").
             FirstOrDefaultAsync();

            usuario.UsuarioFechaLimite = today.AddDays(Int32.Parse(diasPass.ParametroValorInicial));
            _context.Entry(usuario).State = EntityState.Modified;

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

            return Ok(usuario);
        }

        //[HttpPut("{id}")]
        //[HttpPost("login")]
        [HttpPost]
        [Route("SetPassword")]
        public async Task<IActionResult> SetPassword(Usuario usuario)
        {

            var usuarioSave = await _context.Usuarios.FindAsync(usuario.UsuarioId);

            if (usuarioSave == null)
            {
                return BadRequest();
            }


            //if (usuarioSave.UsuarioFechaLimite
            //    == null)
            //{
            //    return BadRequest();
            //}


            DateTime today = DateTime.Now;

            var diasPass = await _context.Parametros.
              Where(u => u.ParametroClave.ToLower() == "SETEXP").
              FirstOrDefaultAsync();

            usuarioSave.UsuarioFechaLimite = today.AddDays(Int32.Parse(diasPass.ParametroValorInicial));

            //usuarioSave.UsuarioFechaLimite = null;
            //usuarioSave.usuario = usuario.Password;
            usuarioSave.Password = usuario.Password;
            _context.Entry(usuarioSave).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.UsuarioId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(usuario);
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            Respuesta respuesta = new Respuesta();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioClave = await _context.Usuarios.
             Where(u => u.UsuarioClave.ToLower() == usuario.UsuarioClave.ToLower()).
             FirstOrDefaultAsync();

            if (usuarioClave != null)
            {
                respuesta.Mensaje = "El nombre de usuario que ingreso ya está registrado.";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            var usuarioEmail = await _context.Usuarios.
               Where(u => u.Email.ToLower() == usuario.Email.ToLower()).
               FirstOrDefaultAsync();

            if (usuarioEmail != null)
            {
                respuesta.Mensaje = "La cuenta de email que ingreso ya está registrada.";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

           

            //if (usuarioEmail == null)
            //{
            //    return BadRequest("No existe la cuenta de email registrada");
            //}

            if (usuario.ImageArray != null && usuario.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(usuario.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "uploads";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);
                if (response)
                {
                    usuario.ImagePath = fullPath;
                }
            }

            if (usuario.ImageBase64 != null && usuario.ImageBase64.Length > 0)
            {
                byte[] imageArray = Convert.FromBase64String(usuario.ImageBase64);
                var stream = new MemoryStream(imageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "uploads";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);
                if (response)
                {
                    usuario.ImagePath = fullPath;
                }
            }

            DateTime today = DateTime.Now;

            var diasPass = await _context.Parametros.
              Where(u => u.ParametroClave.ToLower() == "SETEXP").
              FirstOrDefaultAsync();

            usuario.UsuarioFechaLimite = today.AddDays(Int32.Parse(diasPass.ParametroValorInicial));
            //usuarioEmail.Password = usuario.Password;
            _context.Usuarios.Add(usuario);
            //await _context.SaveChangesAsync();


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.UsuarioId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //Respuesta respuesta = new Respuesta();
            //respuesta.Exito = 1;
            //respuesta.Data = usuario.Email;
            //return Ok(respuesta);
            //UsersHelper.CreateUserASP(user.Email, "User", user.Password);
            //return CreatedAtRoute("DefaultApi", new { id = usuario.UsuarioId }, usuario);
            //_context.Usuarios.Add(usuario);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);

            respuesta.Data = CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);

            respuesta.Mensaje = "Registro exitoso.";
            respuesta.Exito = 1;

            return Ok(respuesta);
        }

        [HttpPost("EmpledosMasivo")]
        public async Task<ActionResult<Usuario>> EmpledosMasivoAsync(List<Usuario> usuarios)
        {
            //Se crea una variable del tipo de servicio para poder decifrar la contraseña
            CifradoHelper cifradoHelper = new CifradoHelper();

            Respuesta respuesta = new Respuesta();
            int contadorNoEmp = 0;
            int contadorEmail= 0;
            int contadorNoEmpSinRegistro = 0;

            string noEmp = "";
            string email = "";
            string noEmpSinRegistro = "";

            foreach (var usuario in usuarios)
            {
                 var EmpleadoNoEmp = await _context.Usuarios.Where(u => u.EmpleadoNoEmp == usuario.EmpleadoNoEmp && u.EmpresaId == usuario.EmpresaId && u.UsuarioEstatusSesion == false).
                 FirstOrDefaultAsync();
                    respuesta.Exito = 1;
                    if (EmpleadoNoEmp != null)
                    {
                        //respuesta.Mensaje = "El número de empleado ya esta registrado, actualicelo desde el sistema o asigne un nuevo número de empleado.";
                        respuesta.Exito = 0;
                        contadorNoEmp += 1;
                        noEmp += EmpleadoNoEmp.EmpleadoNoEmp + ", EmpresaId:" + EmpleadoNoEmp.EmpresaId + ", ";
                        //return Ok(respuesta);
                    }

                    //var usuarioEmail = await _context.Usuarios.
                    //   Where(u => u.Email.ToLower() == usuario.Email.ToLower() && u.UsuarioEstatusSesion == false).
                    //   FirstOrDefaultAsync();

                    //if (usuarioEmail != null)
                    //{
                    //    //respuesta.Mensaje = "La cuenta de email que ingreso ya está registrada.";
                    //    respuesta.Exito = 0;
                    //    contadorEmail += 1;
                    //email += usuarioEmail.Email + ", ";
                    //    //return Ok(respuesta);
                    //}

                    if(respuesta.Exito == 1)
                    {
                    DateTime today = DateTime.Now;

                    var diasPass = await _context.Parametros.
                      Where(u => u.ParametroClave.ToLower() == "SETEXP").
                      FirstOrDefaultAsync();

                    usuario.UsuarioFechaLimite = today.AddDays(Int32.Parse(diasPass.ParametroValorInicial));
                    //usuarioEmail.Password = usuario.Password;

                    usuario.Password = cifradoHelper.EncryptStringAES("UsuarioTmF28");

                    _context.Usuarios.Add(usuario);
                    //await _context.SaveChangesAsync();


                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception es)
                    {
                        //respuesta.Mensaje = "El número de empleado ya esta registrado, actualicelo desde el sistema o asigne un nuevo número de empleado.";
                        respuesta.Exito = 0;
                        contadorNoEmpSinRegistro += 1;
                        noEmpSinRegistro += usuario.EmpleadoNoEmp + ", ";
                    }
                }
                //Respuesta respuesta = new Respuesta();
                //respuesta.Exito = 1;
                //respuesta.Data = usuario.Email;
                //return Ok(respuesta);
                //UsersHelper.CreateUserASP(user.Email, "User", user.Password);
                //return CreatedAtRoute("DefaultApi", new { id = usuario.UsuarioId }, usuario);
                //_context.Usuarios.Add(usuario);
                //await _context.SaveChangesAsync();

                //return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);

                //respuesta.Data = CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);

                //respuesta.Mensaje = "Registro exitoso.";
                //respuesta.Exito = 1;
            }
            respuesta.Exito = 1;
            //int contadorNoEmp = 0;
            //int contadorEmail = 0;
            //int contadorNoEmpSinRegistro = 0;

            if (contadorNoEmp > 0)
            {
                var quitarComaNoEmp = noEmp.Substring(0, noEmp.Length - 2);
                respuesta.Mensaje += "Los siguientes números de empleado ya están registrados, debe actualizarlos: " + quitarComaNoEmp + ". ";
                respuesta.Exito = 0;
            }

            if (contadorEmail > 0)
            {
                var quitarComaEmail = email.Substring(0, email.Length - 2);
                respuesta.Mensaje += "Los siguientes email ya están registrados, debe actualizarlos: " + quitarComaEmail + ". ";
                respuesta.Exito = 0;
            }

            if (contadorNoEmpSinRegistro > 0)
            {
                var quitarComanoEmpSinRegistro = noEmpSinRegistro.Substring(0, noEmpSinRegistro.Length - 2);
                respuesta.Mensaje += "Los siguientes números de empleado no se pudieron registrar, contacte al administrador: " + quitarComanoEmpSinRegistro + ". ";
                respuesta.Exito = 0;
            }

            if (respuesta.Exito == 1)
            {
                respuesta.Mensaje = "Todos los empleados se registraron con éxito.";
            }
            //else
            //{
            //    respuesta.Mensaje = "Registro exitoso.";
            //    respuesta.Exito = 1;
            //}


            return Ok(respuesta);
        }

        // DELETE: api/Usuarios/5
        //[HttpDelete("{id}")]
        //[Authorize]
        //public async Task<ActionResult<Usuario>> DeleteUsuario(int id)
        //{
        //    var usuario = await _context.Usuarios.FindAsync(id);
        //    if (usuario == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Usuarios.Remove(usuario);
        //    await _context.SaveChangesAsync();

        //    return usuario;
        //}

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
