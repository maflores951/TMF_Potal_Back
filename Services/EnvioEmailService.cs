using Fintech.API.Helpers;
using LoginBase.Models;
using LoginBase.Models.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using tmf_group.Models;

namespace LoginBase.Services
{
    public class EnvioEmailService
    {
        private readonly DataContext _db;
        private readonly IWebHostEnvironment _enviroment;

        public EnvioEmailService(DataContext db, IWebHostEnvironment env)
        {
            _db = db;
            _enviroment = env;
        }
        public async Task<Respuesta> EnivarEmail(RecuperaPassParametro userEmail)
        {
            try
            {
                CifradoHelper cifradoHelper = new CifradoHelper();


                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroEmail = await ParametroHelper.RecuperaParametro("smptem",_db);

                //Se valida si existe el parametro
                if (parametroEmail == null)
                {
                    return null;
                }

                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroPass = await ParametroHelper.RecuperaParametro("smptpa", _db);

                //Se valida si existe el parametro
                if (parametroPass == null)
                {
                    return null;
                }
                var CredentialEmail = parametroEmail.ParametroValorInicial;
                var CredentialPassword = parametroPass.ParametroValorInicial;
                var EmailCifrado = cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(userEmail));

                var smtpcl = await ParametroHelper.RecuperaParametro("smptcl", _db);
                MailMessage mail = new MailMessage();
               

                var parametroSubject = await ParametroHelper.RecuperaParametro("smptsu", _db);

                //Se valida si existe el parametro
                if (parametroSubject == null)
                {
                    return null;
                }

                var parametroBody = await ParametroHelper.RecuperaParametro("smptbo", _db);

                //Se valida si existe el parametro
                if (parametroBody == null)
                {
                    return null;
                }

                
                var SMPTPU = await ParametroHelper.RecuperaParametro("SMPTPU", _db);

                var SMPTAU = await ParametroHelper.RecuperaParametro("SMPTAU", _db);

                if (SMPTAU.ParametroValorInicial.Equals("1"))
                {
                    //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                    SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);
                    //Se arma el mensaje con los parametros recuperados
                    mail.From = new MailAddress(CredentialEmail);
                    mail.To.Add(userEmail.Email);
                    mail.Subject = parametroSubject.ParametroValorInicial;
                    mail.Body = parametroBody.ParametroValorInicial + EmailCifrado.Trim().Replace("/", "$").Replace("+", "&");

                    SmtpServer.Port = Int32.Parse(SMPTPU.ParametroValorInicial);//587;
                    SmtpServer.Host = smtpcl.ParametroValorInicial;// "SMTP.Office365.com";
                    SmtpServer.EnableSsl = true;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new NetworkCredential(CredentialEmail, CredentialPassword);
                    //Se envia el correo 
                    SmtpServer.Send(mail);
                }
                else
                {
                    string to = userEmail.Email;
                    string from = CredentialEmail;
                    string subject = parametroSubject.ParametroValorInicial; 
                    string body = parametroBody.ParametroValorInicial + EmailCifrado.Trim().Replace("/", "$").Replace("+", "&");

                    MailMessage message = new MailMessage(from, to, subject, body);
                    SmtpClient client = new SmtpClient(smtpcl.ParametroValorInicial, Int32.Parse(SMPTPU.ParametroValorInicial));
                    // Credentials are necessary if the server requires the client
                    // to authenticate before it will send email on the client's behalf.
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.Send(message);
                }
               


               

                //Se retorna una respuesta correcta
                return new Respuesta
                {
                    Exito = 1,
                    //Message = messageJs
                };
            }
            catch (Exception ex)
            {
                //Se retorna una respuesta incorrecta
                return new Respuesta
                {
                    Exito = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<Respuesta> EnivarEmailNuevaCuenta(RecuperaPassParametro userEmail)
        {
            try
            {
                CifradoHelper cifradoHelper = new CifradoHelper();


                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroEmail = await ParametroHelper.RecuperaParametro("smptem", _db);

                //Se valida si existe el parametro
                if (parametroEmail == null)
                {
                    return null;
                }

                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroPass = await ParametroHelper.RecuperaParametro("smptpa", _db);

                //Se valida si existe el parametro
                if (parametroPass == null)
                {
                    return null;
                }
                var CredentialEmail = parametroEmail.ParametroValorInicial;
                var CredentialPassword = parametroPass.ParametroValorInicial;
                var EmailCifrado = cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(userEmail));

                var smtpcl = await ParametroHelper.RecuperaParametro("smptcl", _db);
                MailMessage mail = new MailMessage();


                var parametroSubject = await ParametroHelper.RecuperaParametro("smptsn", _db);

                //Se valida si existe el parametro
                if (parametroSubject == null)
                {
                    return null;
                }

                var parametroBody = await ParametroHelper.RecuperaParametro("smptbn", _db);

                //Se valida si existe el parametro
                if (parametroBody == null)
                {
                    return null;
                }


                var SMPTPU = await ParametroHelper.RecuperaParametro("SMPTPU", _db);

                var SMPTAU = await ParametroHelper.RecuperaParametro("SMPTAU", _db);

                if (SMPTAU.ParametroValorInicial.Equals("1"))
                {
                    //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                    SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);
                    //Se arma el mensaje con los parametros recuperados
                    mail.From = new MailAddress(CredentialEmail);
                    mail.To.Add(userEmail.Email);
                    mail.Subject = parametroSubject.ParametroValorInicial;
                    mail.Body = parametroBody.ParametroValorInicial + EmailCifrado.Trim().Replace("/", "$").Replace("+", "&");

                    SmtpServer.Port = Int32.Parse(SMPTPU.ParametroValorInicial);//587;
                    SmtpServer.Host = smtpcl.ParametroValorInicial;// "SMTP.Office365.com";
                    SmtpServer.EnableSsl = true;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new NetworkCredential(CredentialEmail, CredentialPassword);
                    //Se envia el correo 
                    SmtpServer.Send(mail);
                }
                else
                {
                    string to = userEmail.Email;
                    string from = CredentialEmail;
                    string subject = parametroSubject.ParametroValorInicial;
                    string body = parametroBody.ParametroValorInicial + EmailCifrado.Trim().Replace("/", "$").Replace("+", "&");

                    MailMessage message = new MailMessage(from, to, subject, body);
                    SmtpClient client = new SmtpClient(smtpcl.ParametroValorInicial, Int32.Parse(SMPTPU.ParametroValorInicial));
                    // Credentials are necessary if the server requires the client
                    // to authenticate before it will send email on the client's behalf.
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.Send(message);
                }





                //Se retorna una respuesta correcta
                return new Respuesta
                {
                    Exito = 1,
                    //Message = messageJs
                };
            }
            catch (Exception ex)
            {
                //Se retorna una respuesta incorrecta
                return new Respuesta
                {
                    Exito = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<Respuesta> EnivarRecibo(EnviarRecibo userEmail)
        {
            try
            {
                //CifradoHelper cifradoHelper = new CifradoHelper();


                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroEmail = await ParametroHelper.RecuperaParametro("smptem", _db);

                //Se valida si existe el parametro
                if (parametroEmail == null)
                {
                    return null;
                }

                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroPass = await ParametroHelper.RecuperaParametro("smptpa", _db);

                //Se valida si existe el parametro
                if (parametroPass == null)
                {
                    return null;
                }
                var CredentialEmail = parametroEmail.ParametroValorInicial;
                var CredentialPassword = parametroPass.ParametroValorInicial;
                //var EmailCifrado = cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(userEmail));

                var smtpcl = await ParametroHelper.RecuperaParametro("smptcl", _db);
                MailMessage mail = new MailMessage();


                var parametroSubject = await ParametroHelper.RecuperaParametro("smptsm", _db);

                //Se valida si existe el parametro
                if (parametroSubject == null)
                {
                    return null;
                }

                var parametroBody = await ParametroHelper.RecuperaParametro("smptmb", _db);

                //Se valida si existe el parametro
                if (parametroBody == null)
                {
                    return null;
                }


                var SMPTPU = await ParametroHelper.RecuperaParametro("SMPTPU", _db);

                var SMPTAU = await ParametroHelper.RecuperaParametro("SMPTAU", _db);

                if (SMPTAU.ParametroValorInicial.Equals("1"))
                {
                    //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                    SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);
                    //Se agrega el archivo
                    // Se crea elarchivo para ser adjuntado
                    Attachment data = new Attachment(userEmail.PathRecibo, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(userEmail.PathRecibo);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(userEmail.PathRecibo);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(userEmail.PathRecibo);
                    // Se agrega el archivo
                    mail.Attachments.Add(data);

                    //Se arma el mensaje con los parametros recuperados
                    mail.From = new MailAddress(CredentialEmail);
                    
                    mail.To.Add(userEmail.Email);
                    mail.Subject = parametroSubject.ParametroValorInicial;
                    mail.Body = parametroBody.ParametroValorInicial;
                   

                    SmtpServer.Port = Int32.Parse(SMPTPU.ParametroValorInicial);//587;
                    SmtpServer.Host = smtpcl.ParametroValorInicial;// "SMTP.Office365.com";
                    SmtpServer.EnableSsl = true;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new NetworkCredential(CredentialEmail, CredentialPassword);
                    //Se envia el correo 
                    SmtpServer.Send(mail);
                }
                else
                {
                    

                    string to = userEmail.Email;
                    string from = CredentialEmail;
                    string subject = parametroSubject.ParametroValorInicial;
                    string body = parametroBody.ParametroValorInicial;

                    MailMessage message = new MailMessage(from, to, subject, body);
                    SmtpClient client = new SmtpClient(smtpcl.ParametroValorInicial, Int32.Parse(SMPTPU.ParametroValorInicial));

                    //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                    SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);
                    //Se agrega el archivo
                    // Se crea elarchivo para ser adjuntado
                    Attachment data = new Attachment(userEmail.PathRecibo, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(userEmail.PathRecibo);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(userEmail.PathRecibo);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(userEmail.PathRecibo);
                    // Se agrega el archivo
                    message.Attachments.Add(data);
                    // Credentials are necessary if the server requires the client
                    // to authenticate before it will send email on the client's behalf.
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.Send(message);
                }

                //Se retorna una respuesta correcta
                return new Respuesta
                {
                    Exito = 1,
                    //Message = messageJs
                };
            }
            catch (Exception ex)
            {
                //Se retorna una respuesta incorrecta
                return new Respuesta
                {
                    Exito = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<Respuesta> EnivarNotificacion(EnviarRecibo userEmail)
        {
            try
            {
                //CifradoHelper cifradoHelper = new CifradoHelper();


                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroEmail = await ParametroHelper.RecuperaParametro("smptem", _db);

                //Se valida si existe el parametro
                if (parametroEmail == null)
                {
                    return null;
                }

                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroPass = await ParametroHelper.RecuperaParametro("smptpa", _db);

                //Se valida si existe el parametro
                if (parametroPass == null)
                {
                    return null;
                }
                var CredentialEmail = parametroEmail.ParametroValorInicial;
                var CredentialPassword = parametroPass.ParametroValorInicial;
                //var EmailCifrado = cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(userEmail));

                var smtpcl = await ParametroHelper.RecuperaParametro("smptcl", _db);
                MailMessage mail = new MailMessage();


                var parametroSubject = await ParametroHelper.RecuperaParametro("smptsm", _db);

                //Se valida si existe el parametro
                if (parametroSubject == null)
                {
                    return null;
                }

                var parametroBody = await ParametroHelper.RecuperaParametro("smptmb", _db);

                //Se valida si existe el parametro
                if (parametroBody == null)
                {
                    return null;
                }


                var SMPTPU = await ParametroHelper.RecuperaParametro("SMPTPU", _db);

                var SMPTAU = await ParametroHelper.RecuperaParametro("SMPTAU", _db);

                if (SMPTAU.ParametroValorInicial.Equals("1"))
                {
                    //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                    SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);
                    ////Se agrega el archivo
                    //// Se crea elarchivo para ser adjuntado
                    //Attachment data = new Attachment(userEmail.PathRecibo, MediaTypeNames.Application.Octet);
                    //ContentDisposition disposition = data.ContentDisposition;
                    //disposition.CreationDate = System.IO.File.GetCreationTime(userEmail.PathRecibo);
                    //disposition.ModificationDate = System.IO.File.GetLastWriteTime(userEmail.PathRecibo);
                    //disposition.ReadDate = System.IO.File.GetLastAccessTime(userEmail.PathRecibo);
                    //// Se agrega el archivo
                    //mail.Attachments.Add(data);

                    //Se arma el mensaje con los parametros recuperados
                    mail.From = new MailAddress(CredentialEmail);

                    mail.To.Add(userEmail.Email);
                    mail.Subject = parametroSubject.ParametroValorInicial;
                    mail.Body = parametroBody.ParametroValorInicial;


                    SmtpServer.Port = Int32.Parse(SMPTPU.ParametroValorInicial);//587;
                    SmtpServer.Host = smtpcl.ParametroValorInicial;// "SMTP.Office365.com";
                    SmtpServer.EnableSsl = true;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new NetworkCredential(CredentialEmail, CredentialPassword);
                    //Se envia el correo 
                    SmtpServer.Send(mail);
                }
                else
                {


                    string to = userEmail.Email;
                    string from = CredentialEmail;
                    string subject = parametroSubject.ParametroValorInicial;
                    string body = parametroBody.ParametroValorInicial;

                    MailMessage message = new MailMessage(from, to, subject, body);
                    SmtpClient client = new SmtpClient(smtpcl.ParametroValorInicial, Int32.Parse(SMPTPU.ParametroValorInicial));

                    //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                    SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);
                    ////Se agrega el archivo
                    //// Se crea elarchivo para ser adjuntado
                    //Attachment data = new Attachment(userEmail.PathRecibo, MediaTypeNames.Application.Octet);
                    //ContentDisposition disposition = data.ContentDisposition;
                    //disposition.CreationDate = System.IO.File.GetCreationTime(userEmail.PathRecibo);
                    //disposition.ModificationDate = System.IO.File.GetLastWriteTime(userEmail.PathRecibo);
                    //disposition.ReadDate = System.IO.File.GetLastAccessTime(userEmail.PathRecibo);
                    //// Se agrega el archivo
                    //message.Attachments.Add(data);
                    // Credentials are necessary if the server requires the client
                    // to authenticate before it will send email on the client's behalf.
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    client.Send(message);
                }

                //Se retorna una respuesta correcta
                return new Respuesta
                {
                    Exito = 1,
                    //Message = messageJs
                };
            }
            catch (Exception ex)
            {
                //Se retorna una respuesta incorrecta
                return new Respuesta
                {
                    Exito = 0,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<Respuesta> EnivarNotificacionMasivo(Recibo model)
        {
            try
            {
                //CifradoHelper cifradoHelper = new CifradoHelper();
                //Se crea la respuesta para el front
                Respuesta respuesta = new Respuesta();
                //Variable para enviar el email
                var email = string.Empty;
                //EnviarRecibo userEmail;

                //var folder = "uploads\\Nomina";

                var contador = 0;
                var contadorEmpNo = "";


                //Se valida que el número de empleado exista 
                var recibos = await _db.Recibos.Where(u =>
                u.EmpresaId == model.EmpresaId &&
                u.PeriodoTipoId == model.PeriodoTipoId &&
                u.ReciboPeriodoA == model.ReciboPeriodoA &&
                u.ReciboPeriodoM == model.ReciboPeriodoM &&
                u.ReciboPeriodoNumero == model.ReciboPeriodoNumero).ToListAsync();

                if (recibos == null)
                {
                    respuesta.Mensaje = "No existen registros para este periodo y empresa";
                    respuesta.Exito = 0;
                    return respuesta;
                }

                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                var parametroEmail = await ParametroHelper.RecuperaParametro("smptae", _db);

                //Se valida si existe el parametro
                if (parametroEmail == null)
                {
                    respuesta.Mensaje = "Error en el parametro 'smptem', contacte al administrador";
                    respuesta.Exito = 0;
                    return respuesta;
                }

                var emails = JsonConvert.DeserializeObject<List<EmailModel>>(parametroEmail.ParametroValorInicial);//parametroEmail.ParametroValorInicial;

                var tamanio = 0;
                var contadorVuelta = 1;
                var numeroVuelta = 0;
                var contadorRegistro = 0;
                var residuo = 0;
                var contadorEmail = 0;

                if (recibos.Count > emails.Count)
                {
                    tamanio = recibos.Count / emails.Count;
                    //if (tamanio > 1)
                    //{
                    if (recibos.Count % tamanio == 0)
                    {
                        numeroVuelta = recibos.Count / tamanio;
                    }
                    else
                    {
                        residuo = recibos.Count % tamanio;
                        numeroVuelta = (recibos.Count / tamanio) + 1;
                    }
                }
                else
                {
                    tamanio = recibos.Count;
                }

                //foreach (var item in emails)
                //{
                //    var prueba = emails.Count;
                //}
                ////Se busca la información del parametro en la tabla Parametros por medio de la clave
                //var parametroPass = await ParametroHelper.RecuperaParametro("smptpa", _db);

                ////Se valida si existe el parametro
                //if (parametroPass == null)
                //{
                //    respuesta.Mensaje = "Error en el parametro 'smptpa', contacte al administrador";
                //    respuesta.Exito = 0;
                //    return respuesta;
                //}

                var CredentialEmail = emails[contadorEmail].Email; //parametroEmail.ParametroValorInicial;
                var CredentialPassword = emails[contadorEmail].Pass;//parametroPass.ParametroValorInicial;
                //var EmailCifrado = cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(userEmail));

                var smtpcl = await ParametroHelper.RecuperaParametro("smptcl", _db);
                //Se valida si existe el parametro
                if (smtpcl == null)
                {
                    respuesta.Mensaje = "Error en el parametro 'smptcl', contacte al administrador";
                    respuesta.Exito = 0;
                    return respuesta;
                }

                MailMessage mail = new MailMessage();

                var parametroSubject = await ParametroHelper.RecuperaParametro("smptsm", _db);


                //Se valida si existe el parametro
                if (parametroSubject == null)
                {
                    respuesta.Mensaje = "Error en el parametro 'smptsm', contacte al administrador";
                    respuesta.Exito = 0;
                    return respuesta;
                }

                var parametroBody = await ParametroHelper.RecuperaParametro("smptmb", _db);

                //Se valida si existe el parametro
                if (parametroBody == null)
                {
                    respuesta.Mensaje = "Error en el parametro 'smptmb', contacte al administrador";
                    respuesta.Exito = 0;
                    return respuesta;
                }


                var SMPTPU = await ParametroHelper.RecuperaParametro("SMPTPU", _db);

                //Se valida si existe el parametro
                if (SMPTPU == null)
                {
                    respuesta.Mensaje = "Error en el parametro 'SMPTPU', contacte al administrador";
                    respuesta.Exito = 0;
                    return respuesta;
                }

                var SMPTAU = await ParametroHelper.RecuperaParametro("SMPTAU", _db);

                //Se valida si existe el parametro
                if (SMPTAU == null)
                {
                    respuesta.Mensaje = "Error en el parametro 'SMPTAU', contacte al administrador";
                    respuesta.Exito = 0;
                    return respuesta;
                }

                if (SMPTAU.ParametroValorInicial.Equals("1"))
                {
                    foreach (var recibo in recibos)
                    {
                        contadorRegistro += 1;
                        //Se busca la información del usuario en la tabla Users por medio del email
                        var usuario = await _db.Usuarios.
                           Where(u => u.EmpleadoNoEmp.ToLower() == recibo.UsuarioNoEmp.ToLower()).
                           FirstOrDefaultAsync();

                        if (usuario == null)
                        {
                            respuesta.Mensaje = "Error #1, contacte al administrador del sistema.";
                            respuesta.Exito = 0;
                            return respuesta;
                        }


                        //userEmail = new EnviarRecibo
                        //{
                        //    Email = usuario.Email,
                        //    PathRecibo = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathPDF)
                        //};

                        ////Se envia el email si todo es correcto
                        //EnvioEmailService enviarEmail = new(_context);
                        //var emailResponse = await enviarEmail.EnivarRecibo(usuarioEmail);

                        try
                        {
                            if (contadorRegistro == tamanio)
                            {
                                //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                                SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);

                                mail.From = new MailAddress(CredentialEmail);


                                mail.Subject = parametroSubject.ParametroValorInicial;
                                mail.Body = parametroBody.ParametroValorInicial;
                                mail.To.Add(usuario.Email);

                                SmtpServer.Port = Int32.Parse(SMPTPU.ParametroValorInicial);//587;
                                SmtpServer.Host = smtpcl.ParametroValorInicial;// "SMTP.Office365.com";
                                SmtpServer.EnableSsl = true;
                                SmtpServer.UseDefaultCredentials = false;
                                SmtpServer.Credentials = new NetworkCredential(CredentialEmail, CredentialPassword);
                                //Se envia el correo 
                                SmtpServer.Send(mail);
                                mail.To.Clear();
                                contadorVuelta += 1;
                                //if (contadorRegistro == tamanio)
                                //{
                                    contadorRegistro = 0;
                                //}
                                if (contadorVuelta == numeroVuelta)
                                {
                                    if (residuo > 0)
                                    {
                                        tamanio = residuo;
                                    }
                                }

                                if (contadorVuelta < numeroVuelta)
                                {
                                    contadorEmail += 1;
                                    CredentialEmail = emails[contadorEmail].Email;
                                    CredentialPassword = emails[contadorEmail].Pass;
                                }
                              
                            }
                            else
                            {
                                mail.To.Add(usuario.Email);
                            }
                        }
                        catch (Exception)
                        {
                            respuesta.Exito = 0;
                            //respuesta.Data = emailResponse;
                            contador += 1;
                            contadorEmpNo += usuario.EmpleadoNoEmp + ", ";
                            //respuesta.Mensaje = "No se pudo enviar";
                        }
                    }
                }
                else
                {
                    foreach (var recibo in recibos)
                    {
                        contadorRegistro += 1;
                        //Se busca la información del usuario en la tabla Users por medio del email
                        var usuario = await _db.Usuarios.
                           Where(u => u.EmpleadoNoEmp.ToLower() == recibo.UsuarioNoEmp.ToLower()).
                           FirstOrDefaultAsync();

                        if (usuario == null)
                        {
                            respuesta.Mensaje = "Error #1, contacte al administrador del sistema.";
                            respuesta.Exito = 0;
                            return respuesta;
                        }


                        //userEmail = new EnviarRecibo
                        //{
                        //    Email = usuario.Email,
                        //    PathRecibo = Path.Combine(_enviroment.ContentRootPath, folder, recibo.ReciboPathPDF)
                        //};

                        ////Se envia el email si todo es correcto
                        //EnvioEmailService enviarEmail = new(_context);
                        //var emailResponse = await enviarEmail.EnivarRecibo(usuarioEmail);

                        try
                        {
                            if (contadorRegistro == tamanio)
                            {
                                //string to = usuario.Email;
                            //string from = CredentialEmail;
                            //string subject = parametroSubject.ParametroValorInicial;
                            //string body = parametroBody.ParametroValorInicial;

                                mail.From = new MailAddress(CredentialEmail);
                                mail.Subject = parametroSubject.ParametroValorInicial;
                                mail.Body = parametroBody.ParametroValorInicial;
                                mail.To.Add(usuario.Email);

                                //MailMessage message = new MailMessage(from, to, subject, body);
                            SmtpClient client = new SmtpClient(smtpcl.ParametroValorInicial, Int32.Parse(SMPTPU.ParametroValorInicial));

                            //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                            SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);
                            
                            client.Credentials = CredentialCache.DefaultNetworkCredentials;
                                //client.Send(message);
                                client.Send(mail);
                                mail.To.Clear();
                           contadorVuelta += 1;
                            //if (contadorRegistro == tamanio)
                            //{
                            contadorRegistro = 0;
                            //}
                            if (contadorVuelta == numeroVuelta)
                            {
                                if (residuo > 0)
                                {
                                    tamanio = residuo;
                                }
                            }

                                if (contadorVuelta < numeroVuelta)
                                {
                                    contadorEmail += 1;
                                    CredentialEmail = emails[contadorEmail].Email;
                                    CredentialPassword = emails[contadorEmail].Pass;
                                }
                        }
                            else
                        {
                            mail.To.Add(usuario.Email);
                        }
                    }
                        catch (Exception)
                        {
                            respuesta.Exito = 0;
                            //respuesta.Data = emailResponse;
                            contador += 1;
                            contadorEmpNo += usuario.EmpleadoNoEmp + ", ";
                            //respuesta.Mensaje = "No se pudo enviar";
                        }
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

                //Se retorna una respuesta correcta
                return new Respuesta
                {
                    Exito = 1
                    //Mensaje = recibos[0].ToString()//messageJs
                };
            }
            catch (Exception ex)
            {
                //Se retorna una respuesta incorrecta
                return new Respuesta
                {
                    Exito = 0,
                    Mensaje = ex.Message
                };
            }
        }
    }
}
