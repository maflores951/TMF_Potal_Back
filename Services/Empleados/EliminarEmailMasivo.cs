using LoginBase.Models;
using LoginBase.Models.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace tmf_group.Services.Empleados
{
    public class EliminarEmailMasivo
    {
        public async static Task<Respuesta> EliminarEmail(IEnumerable<Usuario> usuarios, DataContext context, IWebHostEnvironment enviroment)
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

                    //Eliminar recibos
                    var folder = "uploads\\Nomina";

                    //Se valida que el número de empleado exista 
                    var recibos = await context.Recibos.Where(u =>
                    u.EmpresaId == usuarioUpdate.EmpresaId &&
                    u.UsuarioNoEmp == usuarioUpdate.EmpleadoNoEmp).ToListAsync();

                    foreach (var recibo in recibos)
                    {
                        var pathReciboPDF = Path.Combine(enviroment.ContentRootPath, folder, recibo.ReciboPathPDF);

                        var pathReciboXML = Path.Combine(enviroment.ContentRootPath, folder, recibo.ReciboPathXML);

                        if (System.IO.File.Exists(pathReciboPDF))
                        {
                            //Se elimina el archivo
                            try
                            {
                                System.IO.File.Delete(pathReciboPDF);
                            }
                            catch (Exception)
                            {
                                respuesta.Mensaje = "Error al eliminar el archivo, intente de nuevo o contacte al administrador del sistema.";
                                respuesta.Exito = 0;
                                //return Ok(respuesta);
                            }
                        }

                        if (System.IO.File.Exists(pathReciboXML))
                        {
                            //Se elimina el archivo
                            try
                            {
                                System.IO.File.Delete(pathReciboXML);
                            }
                            catch (Exception)
                            {
                                respuesta.Mensaje = "Error al eliminar el archivo, intente de nuevo o contacte al administrador del sistema.";
                                respuesta.Exito = 0;
                                //return Ok(respuesta);
                            }
                        }
                        context.Recibos.Remove(recibo);
                    }
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
