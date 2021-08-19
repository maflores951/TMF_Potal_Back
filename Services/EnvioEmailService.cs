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

                var smtpcl = await ParametroHelper.RecuperaParametro("smptcl", _db);
                MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("SMTP.Office365.com");
                SmtpClient SmtpServer = new SmtpClient(smtpcl.ParametroValorInicial);

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

                //Se arma el mensaje con los parametros recuperados
                mail.From = new MailAddress(CredentialEmail);
                mail.To.Add(userEmail.Email);
                mail.Subject = parametroSubject.ParametroValorInicial;
                mail.Body = parametroBody.ParametroValorInicial + EmailCifrado.Trim().Replace("/", "$").Replace("+", "&");

                SmtpServer.Port = Int32.Parse(SMPTPU.ParametroValorInicial);//587;
                SmtpServer.Host = smtpcl.ParametroValorInicial;// "SMTP.Office365.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(CredentialEmail, CredentialPassword);

                //SmtpServer.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                //Se envia el correo 
                
                SmtpServer.Send(mail);

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
