using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using tmf_group.Models;
using Microsoft.AspNetCore.Authorization;
using LoginBase.Models.Response;
using System.IO;
using LoginBase.Helper;
using Microsoft.AspNetCore.Hosting;

namespace tmf_group.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class EmpresasController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly IWebHostEnvironment _enviroment;

        public EmpresasController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;

            _enviroment = env;
        }

        // GET: api/Empresas
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Empresa>>> GetEmpresas()
        {
            //return await _context.Empresas.ToListAsync();
            //return await _context.Roles.ToListAsync();

            var responses = new List<Empresa>();
            var empresas = await _context.Empresas.ToListAsync();

            foreach (var empresa in empresas)
            {
                if (empresa.EmpresaEstatus == false)
                {
                    responses.Add(new Empresa
                    {
                        EmpresaId = empresa.EmpresaId,
                        EmpresaNombre = empresa.EmpresaNombre,
                        EmpresaLogo = empresa.EmpresaLogo,
                        EmpresaColor = empresa.EmpresaColor,
                        EmpresaEstatus = empresa.EmpresaEstatus
                    });
                }
            }

            return Ok(responses);
        }

        // GET: api/Empresas/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Empresa>> GetEmpresa(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);

            if (empresa == null)
            {
                return NotFound();
            }

            return empresa;
        }

        // PUT: api/Empresas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutEmpresa(int id, Empresa empresa)
        {
            if (id != empresa.EmpresaId)
            {
                return BadRequest();
            }

            if (empresa.EmpresaImageBase64 != null && empresa.EmpresaImageBase64.Length > 0)
            {
                byte[] imageArray = Convert.FromBase64String(empresa.EmpresaImageBase64);
                var stream = new MemoryStream(imageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "uploads/empresas";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);
                if (response)
                {
                    empresa.EmpresaLogo = fullPath;
                }
            }

            //var validarUsuarios = false;
            //TODO validar que no existan usuarios registrados activos
            if  (empresa.EmpresaEstatus == true)
            {
                var existenUsusarios = await _context.Usuarios.Where(u => u.UsuarioEstatusSesion == false && u.EmpresaId == empresa.EmpresaId).FirstOrDefaultAsync();

                if (existenUsusarios != null)
                {
                    empresa.EmpresaEstatus = false;
                    return Ok(empresa);
                }
            }

            _context.Entry(empresa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(empresa);
            //return NoContent();
        }

        // POST: api/Empresas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Empresa>> PostEmpresa(Empresa empresa)
        {
            Respuesta respuesta = new Respuesta();

            var empresaNombre = await _context.Empresas.
            Where(u => u.EmpresaNombre.ToLower() == empresa.EmpresaNombre.ToLower() && u.EmpresaEstatus == false).
            FirstOrDefaultAsync();

            if (empresaNombre != null)
            {
                respuesta.Mensaje = "El nombre de la entidad que ingreso ya está registrado.";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            if (empresa.EmpresaImageBase64 != null && empresa.EmpresaImageBase64.Length > 0)
            {
                byte[] imageArray = Convert.FromBase64String(empresa.EmpresaImageBase64);
                var stream = new MemoryStream(imageArray);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var folder = "uploads/empresas";
                var fullPath = string.Format("{0}/{1}", folder, file);
                var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);
                if (response)
                {
                    empresa.EmpresaLogo = fullPath;
                }
            }
            


            _context.Empresas.Add(empresa);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpresaExists(empresa.EmpresaId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            respuesta.Data = CreatedAtAction("GetEmpresa", new { id = empresa.EmpresaId }, empresa);

            respuesta.Mensaje = "Registro exitoso.";
            respuesta.Exito = 1;

            return Ok(respuesta);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetEmpresa", new { id = empresa.EmpresaId }, empresa);
        }

        // DELETE: api/Empresas/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteEmpresa(int id)
        //{
        //    var empresa = await _context.Empresas.FindAsync(id);
        //    if (empresa == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Empresas.Remove(empresa);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool EmpresaExists(int id)
        {
            return _context.Empresas.Any(e => e.EmpresaId == id);
        }
    }
}
