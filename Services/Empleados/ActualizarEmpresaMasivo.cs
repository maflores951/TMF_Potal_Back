using LoginBase.Models;
using LoginBase.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tmf_group.Models.Request;

namespace tmf_group.Services.Empleados
{
    public class ActualizarEmpresaMasivo
    {
        public async static Task<Respuesta> ActualizarEmpresa(IEnumerable<UpdateEmpresaUsuario> usuarios, DataContext context)
        {
            Respuesta respuesta = new()
            {
                Data = null,
                Exito = 1,
                Mensaje = string.Empty
            };

            string empleadosNoExisten = "Los siguientes empleados no están registrados en la base de datos: ";
            string empleadosEmpresaExiste = "El número de empleado ya existe para esta entidad: ";
            string empresaExiste = "La entidad no esta registrada en el sistema: ";
            int contarNoExiste = 0;
            int contarEmpresaExiste = 0;
            int contarEmpresaNoExiste = 0;
            //Stopwatch timeMeasure = new();

            //timeMeasure.Start();

            using (DataContext db = context)
            {

                //var usuariosTotales = await db.Usuarios.Where(u => u.UsuarioEstatusSesion == false).ToListAsync();

                var recibosTotales = await db.Recibos.ToListAsync();

                var empresasTotales = await db.Empresas.ToListAsync();

                foreach (var usuario in usuarios)
                {
                    respuesta.Exito = 1;
                    //Se valida que existe el usuario
                    var usuarioUpdate = await db.Usuarios.Where(u => u.EmpleadoNoEmp == usuario.EmpleadoNoEmp && u.EmpresaId == usuario.EmpresaIdOld).FirstOrDefaultAsync(); //usuariosTotales.Find(u => u.EmpleadoNoEmp == usuario.EmpleadoNoEmp && u.EmpresaId == usuario.EmpresaIdOld);

                    if (usuarioUpdate == null)
                    {
                        respuesta.Exito = 0;
                        empleadosNoExisten += $"{usuario.EmpleadoNoEmp} de la entidad {usuario.EmpresaIdOld} , ";
                        contarNoExiste++;
                    }

                    //Se valida que exista la empresa
                    var existeEmpresa = empresasTotales.Find(u =>  u.EmpresaId == usuario.EmpresaIdNew && u.EmpresaEstatus == false);

                    if (existeEmpresa == null)
                    {
                        respuesta.Exito = 0;
                        empresaExiste += $"{usuario.EmpresaIdNew} , ";
                        contarEmpresaNoExiste++;
                    }

                    //Se valida que no exista ese usaurio para esa empresa
                    var existeUsuarioEmpresa = await db.Usuarios.Where(u => u.EmpleadoNoEmp == usuario.EmpleadoNoEmp && u.EmpresaId == usuario.EmpresaIdNew && u.UsuarioEstatusSesion == false).FirstOrDefaultAsync(); //usuariosTotales.Find(u => u.EmpleadoNoEmp == usuario.EmpleadoNoEmp && u.EmpresaId == usuario.EmpresaIdNew);

                    if (existeUsuarioEmpresa != null)
                    {
                        respuesta.Exito = 0;
                        empleadosEmpresaExiste += $"{usuario.EmpleadoNoEmp} de la entidad {usuario.EmpresaIdNew} , ";
                        contarEmpresaExiste++;
                    }

                    ////Se valida que el correo institucional no exista 
                    //var existeEmailSSO = recibosTotales.Find(u => u.EmailSSO == usuario.EmailSSO && u.UsuarioEstatusSesion == usuario.UsuarioEstatusSesion && u.EmpleadoNoEmp != usuario.EmpleadoNoEmp && u.EmpresaId != usuario.EmpresaId);

                    //if (existeEmailSSO != null)
                    //{
                    //    respuesta.Exito = 0;
                    //    empleadosEmailExiste += $"{usuario.EmpleadoNoEmp} de la empresa {usuario.EmpresaId} , ";
                    //    contarEmailExiste++;
                    //}

                    if (respuesta.Exito == 1)
                    {
                        usuarioUpdate.EmpresaId = usuario.EmpresaIdNew;

                        db.Entry(usuarioUpdate).State = EntityState.Modified;


                        var recibosUpdate = recibosTotales.Where(u => u.UsuarioNoEmp == usuario.EmpleadoNoEmp && u.EmpresaId == usuario.EmpresaIdOld).ToList();

                        foreach (var recibo in recibosUpdate)
                        {
                            recibo.EmpresaId = usuario.EmpresaIdNew;
                            //recibo.UsuarioNoEmp = usuario.EmpleadoNoEmp;
                            db.Entry(recibo).State = EntityState.Modified;
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
                }

               

                if (contarNoExiste > 0)
                {
                    empleadosNoExisten = empleadosNoExisten.Substring(0, empleadosNoExisten.Length - 2) + ".  ";
                }
                else
                {
                    empleadosNoExisten = string.Empty;
                }

                if (contarEmpresaExiste > 0)
                {
                    empleadosEmpresaExiste = empleadosEmpresaExiste.Substring(0, empleadosEmpresaExiste.Length - 2) + ". ";
                }
                else
                {
                    empleadosEmpresaExiste = string.Empty;
                }

                if (contarEmpresaNoExiste > 0)
                {
                    empresaExiste = empresaExiste.Substring(0, empresaExiste.Length - 2) + ". ";
                }
                else
                {
                    empresaExiste = string.Empty;
                }

                respuesta.Mensaje += empleadosNoExisten + empleadosEmpresaExiste + empresaExiste;

            }
            //timeMeasure.Stop();

            //respuesta.Mensaje = $"Tiempo que se tardo la ejecución {timeMeasure.ElapsedMilliseconds}";

            return respuesta;
        }

      
    }
}
