using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginBase.Models;
using tmf_group.Models;
using LoginBase.Models.Response;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using LoginBase.Helper;
using System.Text;
using Ionic.Zip;
using Fintech.API.Helpers;
using LoginBase.Services;

namespace tmf_group.Controllers.Recibos
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecibosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _enviroment;

        public RecibosController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _enviroment = env;
        }

        // GET: api/Recibos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recibo>>> GetRecibos()
        {
            //return await _context.Usuarios.ToListAsync();
            var responses = new List<Recibo>();
            var recibos = await _context.Recibos.ToListAsync();

            foreach (var recibo in recibos)
            {
                if (recibo.ReciboEstatus == true)
                {
                    var usuario = await _context.Usuarios.Where(r => r.EmpleadoNoEmp == recibo.UsuarioNoEmp). FirstOrDefaultAsync();

                    var empresa = await _context.Empresas.FindAsync(recibo.EmpresaId);

                    var periodoTipo = await _context.PeriodoTipos.FindAsync(recibo.PeriodoTipoId);

                    responses.Add(new Recibo
                    {
                       ReciboId = recibo.ReciboId,
                       ReciboPeriodoA = recibo.ReciboPeriodoA,
                        ReciboPeriodoM = recibo.ReciboPeriodoM,
                        ReciboPeriodoD = recibo.ReciboPeriodoD,
                        ReciboEstatus = recibo.ReciboEstatus,
                        PeriodoTipoId = recibo.PeriodoTipoId,
                        ReciboPeriodoNumero = recibo.ReciboPeriodoNumero,
                        ReciboPathPDF = recibo.ReciboPathPDF,
                        ReciboPathXML = recibo.ReciboPathXML,
                        UsuarioNoEmp = recibo.UsuarioNoEmp,
                        EmpresaId = recibo.EmpresaId,
                        Usuario = usuario,
                        Empresa = empresa,
                        PeriodoTipo = periodoTipo
                    });
                }
            }

            return Ok(responses);
            //return await _context.Recibos.ToListAsync();
        }

        // GET: api/Recibos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recibo>> GetRecibo(string id)
        {
            var recibo = await _context.Recibos.FindAsync(id);

            if (recibo == null)
            {
                return NotFound();
            }

            return recibo;
        }

        // PUT: api/Recibos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecibo(string id, Recibo recibo)
        {
            if (id != recibo.ReciboId)
            {
                return BadRequest();
            }

            _context.Entry(recibo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReciboExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Recibos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recibo>> PostRecibo(Recibo recibo)
        {
            _context.Recibos.Add(recibo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReciboExists(recibo.ReciboId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRecibo", new { id = recibo.ReciboId }, recibo);
        }

        //// DELETE: api/Recibos/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteRecibo(string id)
        //{
        //    var recibo = await _context.Recibos.FindAsync(id);
        //    if (recibo == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Recibos.Remove(recibo);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //Login del usuario y creación de token
        [HttpPost("cargarArchivo")]
        public async Task<IActionResult> cargarArchivoAsync(Recibo model)
        {
            CifradoHelper cifradoHelper = new CifradoHelper();
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();

            int contador = 0;
            string empleadosNoRegistrados = "";
            //Proceso para guardar el documento en el servidor, se convierte de base64 a archivo
            byte[] imageArray = Convert.FromBase64String(model.ReciboPathPDF);
            var stream = new MemoryStream(imageArray);
            var guid = Guid.NewGuid().ToString();
            var file = string.Format("{0}.zip", guid);//Nombre con el que se guarda el Zip original
            var folder = "uploads\\Nomina";//Folder donde se guarda el archivo
            var fullPath = string.Format("{0}/{1}", folder, file);
            var path = Path.Combine(_enviroment.ContentRootPath, folder, file);
            Random rnd = new Random();
            var nombreZip = "Zip_" + rnd.Next(1, 10000);
            var pathRecibos = Path.Combine(_enviroment.ContentRootPath, "uploads\\Nomina\\", nombreZip); //Carpeta comodin para descomprimir el Zip
            var response = FilesHelper.UploadPhoto(stream, folder, file, _enviroment);

            //var pathNuevo = Path.Combine(_enviroment.ContentRootPath, folder, "Nomina\\2022\\Empleado_0303.pdf");
            //var pathOriginal = Path.Combine(_enviroment.ContentRootPath, folder, "Nomina\\ejemplo_esp.pdf");
            //var pathGuardar = Path.Combine(_enviroment.ContentRootPath, folder, "Nomina");
            //var extractPath = Path.Combine(_enviroment.ContentRootPath, folder);
            //var extractPathGuardar = Path.Combine(_enviroment.ContentRootPath, folder, "pruebaa.zip");
            //var pathString = Path.Combine(_enviroment.ContentRootPath, folder, "Nomina\\2022");

            //Carpeta en donde se guardan los recibos por cada empresa
            var pathEmpPeriodo = model.Empresa.EmpresaNombre + "\\" + model.ReciboPeriodoA + "\\" + model.ReciboPeriodoM + "\\" + model.PeriodoTipoId + "\\" + model.ReciboPeriodoNumero;
            //Se junta el nombre de la carpeta con la ruta donde se almacena el sistema
            var pathEmpresa = Path.Combine(_enviroment.ContentRootPath, folder, pathEmpPeriodo);
            //Se valida si existe la carpeta
            var existePath = System.IO.Directory.Exists(pathEmpresa);

            if (!existePath)
            {
                //Si no existe se crea la carpeta por empresa
                try
                {
                    System.IO.Directory.CreateDirectory(pathEmpresa);

                    respuesta.Exito = 1;
                }
                catch (Exception es)
                {

                    var mensaje = es.Message;
                    respuesta.Mensaje = es.Message;
                    respuesta.Exito = 0;
                    return Ok(respuesta);
                }
            }
            else
            {
                respuesta.Exito = 1;
            }

            //Si la carptea ya existe el proceso continua
            if (respuesta.Exito == 1)
            {
                //Se valida que el archivo original se crea de forma correcta
                if (System.IO.File.Exists(path))
                {
                    //Se extraen los archivos del Zip
                    try
                    {
                        System.IO.Compression.ZipFile.ExtractToDirectory(path, pathRecibos);
                    }
                    catch (Exception)
                    {

                        respuesta.Exito = 0;
                        respuesta.Mensaje = "Error al desempaquetar el archivo .Zip, si el error persiste contacte al administrador del sistema.";
                        return Ok(respuesta);
                    }


                    var existePathZip = new DirectoryInfo(pathRecibos);
                    var carpetaZip = "";

                    //Se recorre la carpeta en donde se extrajo el Zip para recuperar el nombre
                    foreach (var dir in existePathZip.GetDirectories())
                    {
                        carpetaZip = dir.FullName;
                    }

                    DirectoryInfo di = new DirectoryInfo(carpetaZip);

                    //Se recorre la carpeta descomprimida para poder relacionar cada archivo con cada empleado
                    foreach (var fi in di.GetFiles())
                    {
                        Console.WriteLine(fi.Name);
                        //Se recupera el ultimo guion para poder recuperar el número de empleado
                        var posicionFinal = fi.Name.LastIndexOf('_');
                        //Se extra el número de empleado 
                        var empleadoNoD = fi.Name.Substring(posicionFinal + 1);
                        // Se recupera el tamaño para poder eliminar el dominio
                        var lenght = empleadoNoD.Length;

                        var empleadoNo = empleadoNoD.Substring(0, lenght - 4);
                        //Se recupera el domino para poder cambiar el nombre del archivo
                        var dominio = empleadoNoD.Substring(empleadoNo.Length);


                        //Se crea la carpeta por cada empleado
                        var pathEmpNumero = pathEmpPeriodo + "\\" + empleadoNo;
                        var pathEmpNumeroFull = Path.Combine(_enviroment.ContentRootPath, folder, pathEmpNumero);
                        //Se crea el path del archivo para poderlo copiar a una carpeta para el empleado
                        //var pathEmpNumeroDom = pathEmpPeriodo + "\\" + empleadoNo + "\\" + empleadoNo + dominio;
                        var pathEmpNumeroDom = pathEmpPeriodo + "\\" + empleadoNo + "\\" + cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(model.Empresa.EmpresaNombre + "_" + empleadoNo.ToString() + model.ReciboPeriodoA.ToString() + model.ReciboPeriodoM.ToString() + model.PeriodoTipoId.ToString() + model.ReciboPeriodoNumero.ToString())).Replace("/", "$").Replace("+", "&") + dominio;

                        var pathEmpNumeroFullDom = Path.Combine(_enviroment.ContentRootPath, folder, pathEmpNumeroDom);

                        //Se valida si existe el directorio
                        if (System.IO.Directory.Exists(pathEmpNumeroFull))
                        {
                            //Si la carpeta existe se mueve el documento
                            var pathRecibo = folder + "\\" + nombreZip + "\\" + di.Name;// + fi.Name;
                            var pathReciboFull = Path.Combine(_enviroment.ContentRootPath, pathRecibo, fi.Name);

                            if (System.IO.File.Exists(pathEmpNumeroFullDom))
                            {
                                System.IO.File.Delete(pathEmpNumeroFullDom);
                                System.IO.File.Move(pathReciboFull, pathEmpNumeroFullDom);
                            }
                            else
                            {
                                System.IO.File.Move(pathReciboFull, pathEmpNumeroFullDom);
                            }
                        }
                        else
                        {
                            //Si no existe la carpeta se crea y se mueve el archivo
                            try
                            {
                                System.IO.Directory.CreateDirectory(pathEmpNumeroFull);
                            }
                            catch (Exception)
                            {
                                respuesta.Exito = 0;
                                respuesta.Mensaje = "Error #1 en el sistema, contacte al administrador.";
                                return Ok(respuesta);
                            }


                            var pathRecibo = folder + "\\" + nombreZip + "\\" + di.Name;// + fi.Name;
                            var pathReciboFull = Path.Combine(_enviroment.ContentRootPath, pathRecibo, fi.Name);

                            try
                            {
                                if (System.IO.File.Exists(pathEmpNumeroFullDom))
                                {
                                    System.IO.File.Delete(pathEmpNumeroFullDom);
                                    System.IO.File.Move(pathReciboFull, pathEmpNumeroFullDom);
                                }
                                else
                                {
                                    System.IO.File.Move(pathReciboFull, pathEmpNumeroFullDom);
                                }
                            }
                            catch (Exception)
                            {

                                respuesta.Exito = 0;
                                respuesta.Mensaje = "Error #2 en el sistema, contacte al administrador.";
                                return Ok(respuesta);
                            }


                        }
                    }

                    try
                    {
                        //Se eliminan los archivos sin empaquetar
                        System.IO.Directory.Delete(pathRecibos, true);
                        System.IO.File.Delete(path);
                        //System.IO.File.Delete(pathRecibos);
                    }
                    catch (Exception es)
                    {
                        respuesta.Mensaje = es.Message;
                        respuesta.Exito = 0;
                        return Ok(respuesta);

                    }

                    var pathRecibosFinal = Path.Combine(_enviroment.ContentRootPath, folder, pathEmpPeriodo); //Carpeta comodin para descomprimir el Zip
                    var existePathZipFinal = new DirectoryInfo(pathRecibosFinal);
                    var carpetaZipFinal = "";

                    //Se recorre la carpeta en donde se extrajo el Zip para recuperar el nombre
                    foreach (var dir in existePathZipFinal.GetDirectories())
                    {
                        carpetaZipFinal = dir.FullName;
                        //Se recupera el numero de empleado
                        var empleadoNoFinal = dir.Name;

                        //Se valida que el número de empleado exista 
                        var usuario = await _context.Usuarios.Where(u => u.EmpleadoNoEmp.ToLower() == empleadoNoFinal).FirstOrDefaultAsync();

                        if (usuario == null)
                        {
                            empleadosNoRegistrados += empleadoNoFinal + ", ";
                            respuesta.Exito = 1;
                            contador += 1;
                        }
                        else
                        {
                            //Se empaquetan y cifran las carpetas creadas
                            //Se crean las rutas para poder empaquetar los archivos
                            //var pathReciboZip = pathEmpPeriodo; //+ "\\" + empleadoNoFinal;
                            var pathReciboZipExtraer = carpetaZipFinal;//Path.Combine(_enviroment.ContentRootPath, folder, pathReciboZip);

                            var nombreCifrado = cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(model.Empresa.EmpresaNombre + "_" + empleadoNoFinal.ToString() + model.ReciboPeriodoA.ToString() + model.ReciboPeriodoM.ToString() + model.PeriodoTipoId.ToString() + model.ReciboPeriodoNumero.ToString())).Replace("/", "$").Replace("+", "&");
                            var extractPathGuardar = Path.Combine(_enviroment.ContentRootPath, folder, pathEmpPeriodo, nombreCifrado + ".zip");



                            ////Aqui ya estan los archivos en las carpetas
                            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            //try
                            //{
                            //    if (System.IO.File.Exists(extractPathGuardar))
                            //    {
                            //        System.IO.File.Delete(extractPathGuardar);
                            //        //Se empaquetan y se cifran los archivos
                            //        using (var zip = new Ionic.Zip.ZipFile())
                            //        {
                            //            zip.Password = usuario.EmpleadoRFC.ToUpper().Trim();
                            //            zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                            //            zip.AddDirectory(pathReciboZipExtraer, "");
                            //            zip.Save(extractPathGuardar);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        try
                            //        {
                            //            //Se empaquetan y se cifran los archivos
                            //            using (var zip = new Ionic.Zip.ZipFile())
                            //            {
                            //                zip.Password = usuario.EmpleadoRFC.ToUpper().Trim();
                            //                zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                            //                zip.AddDirectory(pathReciboZipExtraer, "");
                            //                zip.Save(extractPathGuardar);
                            //            }
                            //        }
                            //        catch (Exception)
                            //        {
                            //            respuesta.Mensaje += "Error al generar los archivos zip, valide que todos los empleados cuenten con un RFC registrado o contacte al administrador del sistema.";
                            //            respuesta.Exito = 0;

                            //            return Ok(respuesta);
                            //        }

                            //    }

                            try
                            {
                                //Se valida si existe algun registro relacionado al mismo empleado y al mismo periodo
                                var reciboEmpleado = await _context.Recibos.Where(u => u.EmpresaId == model.EmpresaId && u.PeriodoTipoId == model.PeriodoTipoId && u.ReciboPeriodoA == model.ReciboPeriodoA && u.ReciboPeriodoM == model.ReciboPeriodoM && u.ReciboPeriodoNumero == model.ReciboPeriodoNumero && u.UsuarioNoEmp == usuario.EmpleadoNoEmp).FirstOrDefaultAsync();

                                if (reciboEmpleado == null)
                                {
                                    // Crear recibo
                                    Recibo recibo = new Recibo();

                                    recibo.ReciboId = Guid.NewGuid().ToString();
                                    recibo.ReciboPeriodoA = model.ReciboPeriodoA;
                                    recibo.ReciboPeriodoM = model.ReciboPeriodoM;
                                    recibo.ReciboPeriodoD = model.ReciboPeriodoD;
                                    recibo.ReciboEstatus = true;
                                    recibo.PeriodoTipoId = model.PeriodoTipoId;
                                    recibo.ReciboPeriodoNumero = model.ReciboPeriodoNumero;
                                    //recibo.ReciboPathPDF = Path.Combine(pathEmpPeriodo, nombreCifrado + ".zip");
                                    recibo.ReciboPathPDF = pathEmpPeriodo + "\\" + usuario.EmpleadoNoEmp + "\\" + cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(model.Empresa.EmpresaNombre + "_" + usuario.EmpleadoNoEmp.ToString() + model.ReciboPeriodoA.ToString() + model.ReciboPeriodoM.ToString() + model.PeriodoTipoId.ToString() + model.ReciboPeriodoNumero.ToString())).Replace("/", "$").Replace("+", "&") + ".pdf";
                                    recibo.ReciboPathXML = pathEmpPeriodo + "\\" + usuario.EmpleadoNoEmp + "\\" + cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(model.Empresa.EmpresaNombre + "_" + usuario.EmpleadoNoEmp.ToString() + model.ReciboPeriodoA.ToString() + model.ReciboPeriodoM.ToString() + model.PeriodoTipoId.ToString() + model.ReciboPeriodoNumero.ToString())).Replace("/", "$").Replace("+", "&") + ".xml";
                                    recibo.UsuarioNoEmp = usuario.EmpleadoNoEmp;
                                    recibo.EmpresaId = model.EmpresaId;
                                    //Se crea el registro del recibo
                                    _context.Recibos.Add(recibo);
                                    try
                                    {
                                        await _context.SaveChangesAsync();
                                    }
                                    catch (Exception es)
                                    {
                                        respuesta.Mensaje += es.Message;//"No se pudo crear el registro del recibo";
                                        var iner = es.InnerException;
                                        respuesta.Exito = 0;

                                        return Ok(respuesta);
                                    }

                                }
                                else
                                {
                                    //reciboEmpleado.ReciboPathPDF = Path.Combine(pathEmpPeriodo, nombreCifrado + ".zip");
                                    reciboEmpleado.ReciboPathPDF = pathEmpPeriodo + "\\" + usuario.EmpleadoNoEmp + "\\" + cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(model.Empresa.EmpresaNombre + "_" + usuario.EmpleadoNoEmp.ToString() + model.ReciboPeriodoA.ToString() + model.ReciboPeriodoM.ToString() + model.PeriodoTipoId.ToString() + model.ReciboPeriodoNumero.ToString())).Replace("/", "$").Replace("+", "&") + ".pdf";
                                    reciboEmpleado.ReciboPathXML = pathEmpPeriodo + "\\" + usuario.EmpleadoNoEmp + "\\" + cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(model.Empresa.EmpresaNombre + "_" + usuario.EmpleadoNoEmp.ToString() + model.ReciboPeriodoA.ToString() + model.ReciboPeriodoM.ToString() + model.PeriodoTipoId.ToString() + model.ReciboPeriodoNumero.ToString())).Replace("/", "$").Replace("+", "&") + ".xml";


                                    _context.Entry(reciboEmpleado).State = EntityState.Modified;

                                    try
                                    {
                                        await _context.SaveChangesAsync();
                                    }
                                    catch (Exception es)
                                    {
                                        respuesta.Mensaje += es.Message;//"No se pudo crear el registro del recibo";
                                        var iner = es.InnerException;
                                        respuesta.Exito = 0;

                                        return Ok(respuesta);
                                    }
                                }

                            }
                            catch (Exception es)
                            {
                                respuesta.Mensaje = es.Message;
                                respuesta.Exito = 0;
                                return Ok(respuesta);
                            }

                            //try
                            //{
                            //    //Se eliminan los archivos sin empaquetar
                            //    System.IO.Directory.Delete(pathReciboZipExtraer, true);
                            //}
                            //catch (Exception es)
                            //{
                            //    respuesta.Mensaje = es.Message;
                            //    respuesta.Exito = 0;
                            //    return Ok(respuesta);
                            //}
                        }
                    }
                }
                else
                {
                    respuesta.Exito = 0;
                    respuesta.Mensaje = "Error al guardar el archivo .Zip, intente de nuevo.";
                    return Ok(respuesta);
                }
            }
            else
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "Error al crear la carpeta contenedora, contacte al administrador";
                return Ok(respuesta);
            }
            //if (response)
            //{
            //    var prueba = fullPath;
            //}


            if (contador > 0)
            {
                var quitarComa = empleadosNoRegistrados.Substring(0, empleadosNoRegistrados.Length - 2);
                respuesta.Exito = 0;
                respuesta.Mensaje = "Los siguientes empleados no estan registrados en el sistema : " + quitarComa + ".";
            }
            else
            {
                respuesta.Exito = 1;
                respuesta.Mensaje = "Todos los recibos se registrarón con exito";
            }
            return Ok(respuesta);
        }



        //// DELETE: api/Recibos/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteRecibo(string id)
        //{
        //    var recibo = await _context.Recibos.FindAsync(id);
        //    if (recibo == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Recibos.Remove(recibo);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //Login del usuario y creación de token
        [HttpPost("ValidarArchivo")]
        public async Task<IActionResult> ValidarArchivoAsync(Recibo model)
        {
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();

            //Se valida que el número de empleado exista 
            var recibo = await _context.Recibos.Where(u => u.EmpresaId == model.EmpresaId && u.PeriodoTipoId == model.PeriodoTipoId && u.ReciboPeriodoA == model.ReciboPeriodoA && u.ReciboPeriodoM == model.ReciboPeriodoM && u.ReciboPeriodoNumero == model.ReciboPeriodoNumero).FirstOrDefaultAsync();

            if (recibo == null)
            {
                respuesta.Mensaje = "No existen registros para este periodo y empresa";
                respuesta.Exito = 0;
            }
            else
            {
                respuesta.Mensaje = "Ya existen registros para este periodo y empresa";
                respuesta.Exito = 1;
            }

            return Ok(respuesta);
        }

        //Envio de recibo individual
        [HttpPost("EnviarIndividual")]
        public async Task<IActionResult> EnviarIndividualAsync(Recibo model)
        {
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();
            //Variable para enviar el email
            var email = string.Empty;
            EnviarRecibo usuarioEmail;

            var folder = "uploads\\Nomina";


            //Se valida que el número de empleado exista 
            var recibo = await _context.Recibos.Where(u => 
            u.EmpresaId == model.EmpresaId && 
            u.PeriodoTipoId == model.PeriodoTipoId && 
            u.ReciboPeriodoA == model.ReciboPeriodoA && 
            u.ReciboPeriodoM == model.ReciboPeriodoM && 
            u.ReciboPeriodoNumero == model.ReciboPeriodoNumero && 
            u.UsuarioNoEmp == model.UsuarioNoEmp).FirstOrDefaultAsync();

            if (recibo == null)
            {
                respuesta.Mensaje = "No existen registros para este periodo y empresa";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            //Se busca la información del usuario en la tabla Users por medio del email
            var usuario = await _context.Usuarios.
               Where(u => u.EmpleadoNoEmp.ToLower() == model.UsuarioNoEmp.ToLower()).
               FirstOrDefaultAsync();

            if (usuario == null)
            {
                respuesta.Mensaje = "No existen registros para este usuario, contacte al administrador";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            usuarioEmail = new EnviarRecibo
            {
                Email = usuario.Email,
                PathRecibo = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathPDF)
            };

            //Se envia el email si todo es correcto
            EnvioEmailService enviarEmail = new EnvioEmailService(_context);
            var emailResponse = await enviarEmail.EnivarRecibo(usuarioEmail);

            if (emailResponse.Exito == 1)
            {
                respuesta.Exito = 1;
                respuesta.Data = emailResponse;
                respuesta.Mensaje = "El recibo se envió con éxito.";
            }
            else
            {
                respuesta.Exito = 0;
                respuesta.Data = emailResponse;
                respuesta.Mensaje = "Error en el servidor, contacte al administrador del sistema.";
            }

            return Ok(respuesta);
        }

        //Envio de recibo individual
        [HttpPost("EnviarNotificacion")]
        public async Task<IActionResult> EnviarNotificacionAsync(Recibo model)
        {
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();
            //Variable para enviar el email
            var email = string.Empty;
            EnviarRecibo usuarioEmail;

            //var folder = "uploads\\Nomina";


            //Se valida que el número de empleado exista 
            var recibo = await _context.Recibos.Where(u =>
            u.EmpresaId == model.EmpresaId &&
            u.PeriodoTipoId == model.PeriodoTipoId &&
            u.ReciboPeriodoA == model.ReciboPeriodoA &&
            u.ReciboPeriodoM == model.ReciboPeriodoM &&
            u.ReciboPeriodoNumero == model.ReciboPeriodoNumero &&
            u.UsuarioNoEmp == model.UsuarioNoEmp).FirstOrDefaultAsync();

            if (recibo == null)
            {
                respuesta.Mensaje = "No existen registros para este periodo y empresa";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            //Se busca la información del usuario en la tabla Users por medio del email
            var usuario = await _context.Usuarios.
               Where(u => u.EmpleadoNoEmp.ToLower() == model.UsuarioNoEmp.ToLower()).
               FirstOrDefaultAsync();

            if (usuario == null)
            {
                respuesta.Mensaje = "No existen registros para este usuario, contacte al administrador";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            usuarioEmail = new EnviarRecibo
            {
                Email = usuario.Email,
                //PathRecibo = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathPDF)
            };

            //Se envia el email si todo es correcto
            EnvioEmailService enviarEmail = new EnvioEmailService(_context);
            var emailResponse = await enviarEmail.EnivarNotificacion(usuarioEmail);

            if (emailResponse.Exito == 1)
            {
                respuesta.Exito = 1;
                respuesta.Data = emailResponse;
                respuesta.Mensaje = "El recibo se envió con éxito.";
            }
            else
            {
                respuesta.Exito = 0;
                respuesta.Data = emailResponse;
                respuesta.Mensaje = "Error en el servidor, contacte al administrador del sistema.";
            }

            return Ok(respuesta);
        }


        //Envio de recibo individual
        [HttpPost("EnviarMasivo")]
        public async Task<IActionResult> EnviarMasivoAsync(Recibo model)
        {
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();
            //Variable para enviar el email
            var email = string.Empty;
            EnviarRecibo usuarioEmail;

            var folder = "uploads\\Nomina";

            var contador = 0;
            var contadorEmpNo = "";


            //Se valida que el número de empleado exista 
            var recibos = await _context.Recibos.Where(u =>
            u.EmpresaId == model.EmpresaId &&
            u.PeriodoTipoId == model.PeriodoTipoId &&
            u.ReciboPeriodoA == model.ReciboPeriodoA &&
            u.ReciboPeriodoM == model.ReciboPeriodoM &&
            u.ReciboPeriodoNumero == model.ReciboPeriodoNumero).ToListAsync();

            if (recibos == null)
            {
                respuesta.Mensaje = "No existen registros para este periodo y empresa";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }


            foreach (var recibo in recibos)
            {
                //Se busca la información del usuario en la tabla Users por medio del email
                var usuario = await _context.Usuarios.
                   Where(u => u.EmpleadoNoEmp.ToLower() == recibo.UsuarioNoEmp.ToLower()).
                   FirstOrDefaultAsync();

                if (usuario == null)
                {
                    respuesta.Mensaje = "Error #1, contacte al administrador del sistema.";
                    respuesta.Exito = 0;
                    return Ok(respuesta);
                }

                
                usuarioEmail = new EnviarRecibo
                {
                    Email = usuario.Email,
                    PathRecibo = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathPDF)
                };

                //Se envia el email si todo es correcto
                EnvioEmailService enviarEmail = new(_context);
                var emailResponse = await enviarEmail.EnivarRecibo(usuarioEmail);

                if (emailResponse.Exito == 1)
                {
                    respuesta.Exito = 1;
                    //respuesta.Data = emailResponse;
                }
                else
                {
                    respuesta.Exito = 0;
                    //respuesta.Data = emailResponse;
                    contador += 1;
                    contadorEmpNo += usuario.EmpleadoNoEmp + ", ";
                    //respuesta.Mensaje = "No se pudo enviar";
                }
            }

            if (contador > 0)
            {
                var quitarComa = contadorEmpNo.Substring(0, contadorEmpNo.Length - 2);
                respuesta.Exito = 0;
                respuesta.Mensaje = "Los siguientes empleados no estan registrados en el sistema : " + quitarComa + ".";
            }
            else
            {
                respuesta.Exito = 1;
                respuesta.Mensaje = "Todos los recibos se enviaron con éxito.";
            }


            return Ok(respuesta);
        }

        //Envio de recibo individual
        [HttpPost("NotificarMasivo")]
        public async Task<IActionResult> NotificarMasivoAsync(Recibo model)
        {
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();
            //Variable para enviar el email
            var email = string.Empty;
            EnviarRecibo usuarioEmail;

            var folder = "uploads\\Nomina";

            var contador = 0;
            var contadorEmpNo = "";


            //Se valida que el número de empleado exista 
            var recibos = await _context.Recibos.Where(u =>
            u.EmpresaId == model.EmpresaId &&
            u.PeriodoTipoId == model.PeriodoTipoId &&
            u.ReciboPeriodoA == model.ReciboPeriodoA &&
            u.ReciboPeriodoM == model.ReciboPeriodoM &&
            u.ReciboPeriodoNumero == model.ReciboPeriodoNumero).ToListAsync();

            if (recibos == null)
            {
                respuesta.Mensaje = "No existen registros para este periodo y empresa";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }


            foreach (var recibo in recibos)
            {
                //Se busca la información del usuario en la tabla Users por medio del email
                var usuario = await _context.Usuarios.
                   Where(u => u.EmpleadoNoEmp.ToLower() == recibo.UsuarioNoEmp.ToLower()).
                   FirstOrDefaultAsync();

                if (usuario == null)
                {
                    respuesta.Mensaje = "Error #1, contacte al administrador del sistema.";
                    respuesta.Exito = 0;
                    return Ok(respuesta);
                }


                usuarioEmail = new EnviarRecibo
                {
                    Email = usuario.Email,
                    //PathRecibo = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathPDF)
                };

                //Se envia el email si todo es correcto
                EnvioEmailService enviarEmail = new(_context);
                var emailResponse = await enviarEmail.EnivarNotificacion(usuarioEmail);

                if (emailResponse.Exito == 1)
                {
                    respuesta.Exito = 1;
                    //respuesta.Data = emailResponse;
                }
                else
                {
                    respuesta.Exito = 0;
                    //respuesta.Data = emailResponse;
                    contador += 1;
                    contadorEmpNo += usuario.EmpleadoNoEmp + ", ";
                    //respuesta.Mensaje = "No se pudo enviar";
                }
            }

            if (contador > 0)
            {
                var quitarComa = contadorEmpNo.Substring(0, contadorEmpNo.Length - 2);
                respuesta.Exito = 0;
                respuesta.Mensaje = "Los siguientes empleados no estan registrados en el sistema : " + quitarComa + ".";
            }
            else
            {
                respuesta.Exito = 1;
                respuesta.Mensaje = "Todos los recibos se enviaron con éxito.";
            }


            return Ok(respuesta);
        }

        //Envio de recibo individual
        [HttpPost("BorrarIndividual")]
        public async Task<IActionResult> BorrarIndividualAsync(Recibo model)
        {
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();

            var folder = "uploads\\Nomina";


            //Se valida que el número de empleado exista 
            var recibo = await _context.Recibos.Where(u =>
            u.EmpresaId == model.EmpresaId &&
            u.PeriodoTipoId == model.PeriodoTipoId &&
            u.ReciboPeriodoA == model.ReciboPeriodoA &&
            u.ReciboPeriodoM == model.ReciboPeriodoM &&
            u.ReciboPeriodoNumero == model.ReciboPeriodoNumero &&
            u.UsuarioNoEmp == model.UsuarioNoEmp).FirstOrDefaultAsync();

            if (recibo == null)
            {
                respuesta.Mensaje = "No existen registros para este periodo y empresa";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            ////Se busca la información del usuario en la tabla Users por medio del email
            //var usuario = await _context.Usuarios.
            //   Where(u => u.EmpleadoNoEmp.ToLower() == model.UsuarioNoEmp.ToLower()).
            //   FirstOrDefaultAsync();

            //if (usuario == null)
            //{
            //    respuesta.Mensaje = "No existen registros para este usuario, contacte al administrador";
            //    respuesta.Exito = 0;
            //    return Ok(respuesta);
            //}

            var pathReciboPDF = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathPDF);

            var pathReciboXML = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathXML);

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
                    return Ok(respuesta);
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
                    return Ok(respuesta);
                }
            }


            _context.Recibos.Remove(recibo);

            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                respuesta.Mensaje = "Error al eliminar el registro, intente de nuevo o contacte al administrador del sistema.";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            respuesta.Exito = 1;
            respuesta.Mensaje = "El archivo se eliminó correctamente.";


            return Ok(respuesta);
        }

        //Envio de recibo individual
        [HttpPost("BorrarMasivo")]
        public async Task<IActionResult> BorrarMasivoAsync(Recibo model)
        {
            //Se crea la respuesta para el front
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 1;
            respuesta.Mensaje = "Todos los recibos se eliminaron correctamente.";
            var folder = "uploads\\Nomina";

            var contador = 0;
            var contadorEmpNo = "";


            //Se valida que el número de empleado exista 
            var recibos = await _context.Recibos.Where(u =>
            u.EmpresaId == model.EmpresaId &&
            u.PeriodoTipoId == model.PeriodoTipoId &&
            u.ReciboPeriodoA == model.ReciboPeriodoA &&
            u.ReciboPeriodoM == model.ReciboPeriodoM &&
            u.ReciboPeriodoNumero == model.ReciboPeriodoNumero).ToListAsync();

            if (recibos == null)
            {
                respuesta.Mensaje = "No existen registros para este periodo y empresa";
                respuesta.Exito = 0;
                return Ok(respuesta);
            }

            //Carpeta en donde se guardan los recibos por cada empresa
            var pathEmpPeriodo = model.Empresa.EmpresaNombre + "\\" + model.ReciboPeriodoA + "\\" + model.ReciboPeriodoM + "\\" + model.PeriodoTipoId + "\\" + model.ReciboPeriodoNumero;
            //Se junta el nombre de la carpeta con la ruta donde se almacena el sistema
            var pathEmpresa = Path.Combine(_enviroment.ContentRootPath, folder, pathEmpPeriodo);
            //Se valida si existe la carpeta
            var existePath = System.IO.Directory.Exists(pathEmpresa);


            if (existePath)
            {
                //Se eliminan los archivos sin empaquetar
                try
                {
                    System.IO.Directory.Delete(pathEmpresa, true);
                }
                catch (Exception)
                {
                    respuesta.Mensaje = "Error al eliminar los archivos, intente de nuevo o contacte al administrador del sistema.";
                    respuesta.Exito = 0;
                    return Ok(respuesta);
                }
            }

            foreach (var recibo in recibos)
            {
 
                _context.Recibos.Remove(recibo);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    //respuesta.Mensaje = "Error al eliminar el registro, intente de nuevo o contacte al administrador del sistema.";
                    //respuesta.Exito = 0;
                    //return Ok(respuesta);
                    contador += 1;
                    contadorEmpNo += recibo.UsuarioNoEmp + ", ";
                }

            }

            if (contador > 0)
            {
                var quitarComa = contadorEmpNo.Substring(0, contadorEmpNo.Length - 2);
                respuesta.Exito = 0;
                respuesta.Mensaje = "Los siguientes recibos no se pudieron eliminar, intente de nuevo o contacte al administrador del sistema : " + quitarComa + ".";
            }

            return Ok(respuesta);
        }

        // Recuperar recibos por usuario
        [HttpPost("Usuario")]
        public async Task<IActionResult> UsuarioAsync(Recibo model)
        {
            //return await _context.Usuarios.ToListAsync();
            var responses = new List<Recibo>();
            var recibos = await _context.Recibos.Where(r => r.EmpresaId == model.EmpresaId && r.UsuarioNoEmp == model.UsuarioNoEmp && r.ReciboEstatus == true).ToListAsync();

            var folder = "uploads\\Nomina";

            foreach (var recibo in recibos)
            {
                
                var usuario = await _context.Usuarios.Where(r => r.EmpleadoNoEmp == recibo.UsuarioNoEmp).FirstOrDefaultAsync();

                //var empresa = await _context.Empresas.FindAsync(recibo.EmpresaId);

                var periodoTipo = await _context.PeriodoTipos.FindAsync(recibo.PeriodoTipoId);

                responses.Add(new Recibo
                {
                    ReciboId = recibo.ReciboId,
                    ReciboPeriodoA = recibo.ReciboPeriodoA,
                    ReciboPeriodoM = recibo.ReciboPeriodoM,
                    ReciboPeriodoD = recibo.ReciboPeriodoD,
                    ReciboEstatus = recibo.ReciboEstatus,
                    PeriodoTipoId = recibo.PeriodoTipoId,
                    ReciboPeriodoNumero = recibo.ReciboPeriodoNumero,
                    ReciboPathPDF = Path.Combine(folder,recibo.ReciboPathPDF).Replace("\\", "/"),
                    ReciboPathXML = Path.Combine(folder,recibo.ReciboPathXML).Replace("\\", "/"),
                    UsuarioNoEmp = recibo.UsuarioNoEmp,
                    EmpresaId = recibo.EmpresaId,
                    Usuario = usuario,
                    //Empresa = empresa,
                    PeriodoTipo = periodoTipo
                });
            }

            return Ok(responses);
            //return await _context.Recibos.ToListAsync();
        }

        private bool ReciboExists(string id)
        {
            return _context.Recibos.Any(e => e.ReciboId == id);
        }
    }
}
