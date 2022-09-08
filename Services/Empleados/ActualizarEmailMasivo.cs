using LoginBase.Models;
using LoginBase.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace tmf_group.Services.Empleados
{
    public class ActualizarEmailMasivo
    {
        //private readonly DataContext _context;

        //private ActualizarEmailMasivo(DataContext context)
        //{
        //    _context = context;
        //}

        public async static Task<Respuesta> ActualizarEmail(IEnumerable<Usuario> usuarios, DataContext context)
        {
            Respuesta respuesta = new()
            {
                Data = null,
                Exito = 1,
                Mensaje = string.Empty
            };

            string empleadosNoExisten = "Los siguientes empleados no están registrados en la base de datos: ";
            string empleadosEmailExiste = "El correo electrónico de los siguientes empleados ya están registrados: ";
            int contarNoExiste = 0;
            int contarEmailExiste = 0;
            //Stopwatch timeMeasure = new();

            //timeMeasure.Start();

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
                        
                    //Se valida que el correo no exista 
                    var existeEmail = recibosTotales.Find(u => u.Email == usuario.Email && u.UsuarioEstatusSesion == usuario.UsuarioEstatusSesion && u.EmpleadoNoEmp != usuario.EmpleadoNoEmp && u.EmpresaId != usuario.EmpresaId);

                    if (existeEmail != null)
                    {
                        respuesta.Exito = 0;
                        empleadosEmailExiste += $"{usuario.EmpleadoNoEmp} de la empresa {usuario.EmpresaId} , ";
                        contarEmailExiste++;
                    }

                    //Se valida que el correo institucional no exista 
                    var existeEmailSSO = recibosTotales.Find(u => u.EmailSSO == usuario.EmailSSO && u.UsuarioEstatusSesion == usuario.UsuarioEstatusSesion && u.EmpleadoNoEmp != usuario.EmpleadoNoEmp && u.EmpresaId != usuario.EmpresaId);

                    if (existeEmailSSO != null)
                    {
                        respuesta.Exito = 0;
                        empleadosEmailExiste += $"{usuario.EmpleadoNoEmp} de la empresa {usuario.EmpresaId} , ";
                        contarEmailExiste++;
                    }

                    usuarioUpdate.Email = usuario.Email;

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

                if(contarNoExiste > 0){
                    empleadosNoExisten = empleadosNoExisten.Substring(empleadosNoExisten.Length - 2) + ". ";
                }

                if (contarEmailExiste > 0)
                {
                    empleadosEmailExiste = empleadosEmailExiste.Substring(empleadosEmailExiste.Length - 2) + ".";
                }

                respuesta.Mensaje = empleadosNoExisten + empleadosEmailExiste;

                //var usuariosActualizados = (from usuario in recibosTotales
                //                            join usuarioActualizado in usuarios
                //                           on new { usuario.EmpleadoNoEmp, usuario.EmpresaId } equals new { usuarioActualizado.EmpleadoNoEmp, usuarioActualizado.EmpresaId }
                //                            select new Usuario()
                //                            {
                //                                UsuarioId = usuario.UsuarioId,
                //                                UsuarioApellidoP = usuario.UsuarioApellidoP,
                //                                UsuarioApellidoM = usuario.UsuarioApellidoM,
                //                                UsuarioNombre = usuario.UsuarioNombre,
                //                                Email = usuarioActualizado.Email,
                //                                EmpleadoNoEmp = usuario.EmpleadoNoEmp,
                //                                UsuarioClave = usuario.UsuarioClave,
                //                                EmpresaId = usuario.EmpresaId,
                //                                ImagePath = usuario.ImagePath,
                //                                Password = usuario.Password,
                //                                RolId = usuario.RolId,
                //                                UsuarioEstatusSesion =
                //                                usuario.UsuarioEstatusSesion,
                //                                UsuarioFechaLimite = usuario.UsuarioFechaLimite,
                //                                UsuarioNumConfirmacion = usuario.UsuarioNumConfirmacion,
                //                                UsuarioToken = usuario.UsuarioToken,
                //                                Empresa = usuario.Empresa,
                //                                ImageArray =usuario.ImageArray,
                //                                ImageBase64 = usuario.ImageBase64,
                //                                Rol = usuario.Rol
                //                            });



                //try
                //{
                //    foreach (var usuario in usuariosActualizados)
                //    {

                //        //await db.SaveChangesAsync();
                //        respuesta.Data = usuario;

                //        //usuario.Email = "maflores@cargamasiva.com";
                //        db.Entry(usuario).State = EntityState.Modified;



                //    }
                //    //    //db.Entry(usuariosActualizados).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                //catch (Exception es)
                //{
                //    respuesta.Mensaje = es.Message;
                //    respuesta.Exito = 0;
                //}
            }
            //timeMeasure.Stop();

            //respuesta.Mensaje = $"Tiempo que se tardo la ejecución {timeMeasure.ElapsedMilliseconds}";

            return respuesta;
        }
    }
}
