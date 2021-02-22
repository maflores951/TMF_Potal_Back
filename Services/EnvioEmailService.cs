using Fintech.API.Helpers;
using LoginBase.Models;
using LoginBase.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

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

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");

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

                mail.From = new MailAddress(CredentialEmail);
                mail.To.Add(userEmail.Email);
                mail.Subject = parametroSubject.ParametroValorInicial;
                mail.Body = parametroBody.ParametroValorInicial + EmailCifrado.Trim().Replace("/", "$").Replace("+", "&");

                SmtpServer.Port = 587;
                SmtpServer.Host = "SMTP.Office365.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(CredentialEmail, CredentialPassword);

                SmtpServer.Send(mail);

                return new Respuesta
                {
                    Exito = 1,
                    //Message = messageJs
                };
            }
            catch (Exception ex)
            {
                return new Respuesta
                {
                    Exito = 0,
                    Mensaje = ex.Message
                };
            }
        }
    }
}
