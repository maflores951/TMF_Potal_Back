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
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            //return await _context.Usuarios.ToListAsync();
            var responses = new List<Usuario>();
            var usuarios = await _context.Usuarios.ToListAsync();

            foreach (var usuario in usuarios)
            {
                if(usuario.UsuarioEstatusSesion == false) { 
                var rol = await _context.Roles.FindAsync(usuario.RolId);
                responses.Add(new Usuario
                {
                    UsuarioId = usuario.UsuarioId,
                    UsuarioNombre = usuario.UsuarioNombre,
                    UsuarioApellidoP = usuario.UsuarioApellidoP,
                    UsuarioApellidoM = usuario.UsuarioApellidoM,
                    UsuarioNumConfirmacion = usuario.UsuarioNumConfirmacion,
                    UsuarioFechaLimite = usuario.UsuarioFechaLimite,
                    UsuarioEstatusSesion = usuario.UsuarioEstatusSesion,
                    Password = usuario.Password,
                    Email = usuario.Email,
                    ImagePath = usuario.ImagePath,
                    RolId = usuario.RolId,
                    Rol = usuario.Rol
                });
                }
            }

            return Ok(responses);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
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

            if (usuario.ImageArray != null && usuario.ImageArray.Length > 0)
            {
                var stream = new MemoryStream(usuario.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "~/Content/Images";
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


            if (usuarioSave.UsuarioFechaLimite
                == null)
            {
                return BadRequest();
            }

            usuarioSave.UsuarioFechaLimite = null;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var usuarioEmail = await _context.Usuarios.
               Where(u => u.Email.ToLower() == usuario.Email.ToLower()).
               FirstOrDefaultAsync();

            if (usuarioEmail != null)
            {
                return BadRequest("La cuenta de email que ingreso ya está registrada");
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
                var folder = "~/Content/Images";
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
                var folder = "~/Content/Images";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);
                if (response)
                {
                    usuario.ImagePath = fullPath;
                }
            }
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

            return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Usuario>> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
