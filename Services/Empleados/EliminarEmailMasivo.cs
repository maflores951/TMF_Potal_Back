using LoginBase.Models;
using LoginBase.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tmf_group.Services.Empleados
{
    public class EliminarEmailMasivo
    {
        public async static Task<Respuesta> EliminarEmail(IEnumerable<Usuario> usuarios, DataContext context)
        {
            Respuesta respuesta = new()
            {
                Data = null,
                Exito = 1,
                Mensaje = string.Empty
            };

            string empleadosNoExisten = "Los siguientes empleados no están registrados en la base de datos: ";
            
            int contarNoExiste = 0;
           

            using (DataContext db = context)
            {

                var recibosTotales = await db.Usuarios.Where(u => u.UsuarioEstatusSesion == false).ToListAsync();

                foreach (var usuario in usuarios)
                {
                    //Se valida que existe el usuario
                    var usuarioUpdate = recibosTotales.Find(u => u.EmpleadoNoEmp == usuario.EmpleadoNoEmp && u.EmpresaId == usuario.EmpresaId && u.UsuarioEstatusSesion == usuario.UsuarioEstatusSesion);

                    if (usuarioUpdate == null)
                    {
                        respuesta.Exito = 0;
                        empleadosNoExisten += $"{usuario.EmpleadoNoEmp} de la empresa {usuario.EmpresaId} , ";
                        contarNoExiste++;
                    }

                    usuarioUpdate.UsuarioEstatusSesion = true;

                    db.Entry(usuarioUpdate).State = EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                }
                catch (Exception es)
                {
                    respuesta.Mensaje = es.Message;
                    respuesta.Exito = 0;
                    return respuesta;
                }

                if (contarNoExiste > 0)
                {
                    empleadosNoExisten = empleadosNoExisten.Substring(empleadosNoExisten.Length - 2) + ".";
                }

                respuesta.Mensaje = empleadosNoExisten;
            }
            
            return respuesta;
        }
    }
}
