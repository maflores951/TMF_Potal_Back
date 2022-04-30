using Fintech.API.Helpers;
using LoginBase.Models;
using LoginBase.Models.Response;
using System;
using System.Collections.Generic;
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
        public EnvioEmailService(DataContext db)
        {
            _db = db;
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
                var CredentialEmail = parametroEmail.ParametroValorInicial;//"maflores@legvit.com";
                var CredentialPassword = parametroPass.ParametroValorInicial;//"Sich2017";
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
                var CredentialEmail = parametroEmail.ParametroValorInicial;//"maflores@legvit.com";
                var CredentialPassword = parametroPass.ParametroValorInicial;//"Sich2017";
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
    }
}
