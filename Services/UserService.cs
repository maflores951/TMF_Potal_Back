using Fintech.API.Helpers;
using LoginBase.Models;
using LoginBase.Models.Common;
using LoginBase.Models.Request;
using LoginBase.Models.Response;
using LoginBase.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoginBase.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _db;

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings, DataContext db)
        {
            _appSettings = appSettings.Value;
            _db = db;
        }

        //Se valida la contraseña y usario para el login
        public Respuesta Auth(AuthRequest model)
        {
            //UserResponse userResponse = new UserResponse();
            //Se crea una variable del tipo de servicio para poder cifrar información
            CifradoHelper cifradoHelper = new CifradoHelper();
            Respuesta respuesta = new();

            //Se crea la variable para recuperar el usuario
            Usuario usuarioM = new Usuario();
            using (var db = _db)
            {
                //Se decifra la contraseña enviada por el usuario
                string sPassword = cifradoHelper.DecryptStringAES(model.Password);

                //var usuario = db.Usuarios.FirstOrDefault();
                //var usuario = _db.Usuarios.Where(d => d.Email == model.Email && cifradoHelper.DecryptStringAES(d.Password) == sPassword).FirstOrDefault();

                //Se recupera el usuario con respecto al email
                var usuario = _db.Usuarios.Where(d => d.UsuarioClave == model.Email && d.UsuarioEstatusSesion == false).FirstOrDefault();

                //Si no existe el usuario retorna un null
                if (usuario == null)
                {
                    respuesta.Mensaje = "Usuario o contraseña incorrecta.";
                    respuesta.Exito = 0;
                    return respuesta;
                }

                //Se valida si el usuario tiene un correo institucional relacionado
                if (!usuario.EmailSSO.IsNullOrEmpty())
                {
                    respuesta.Mensaje = "Usuario o contraseña incorrecta, ingrese con su cuenta institucional.";
                    respuesta.Exito = 0;
                    respuesta.Data = null;
                    return respuesta;
                }
               

                ////Se valida si el usuario tiene un correo institucional relacionado
                //if (usuario.EmailSSO != string.Empty)
                //{
                //    respuesta.Mensaje = "Usuario o contraseña incorrecta, ingrese con su cuenta institucional.";
                //    respuesta.Exito = 0;
                //    respuesta.Data = null;
                //    return respuesta;
                //}

                //Si existe el usuario se comparan las contraseñas decifradas
                if (cifradoHelper.DecryptStringAES(usuario.Password) != sPassword)
                {
                    respuesta.Mensaje = "Usuario o contraseña incorrecta.";
                    respuesta.Exito = 0;
                    return respuesta;
                }


               
                //Se crea y se recupera el token de seguridad 
                usuario.UsuarioToken = GetToken(usuario);
                //_db.Entry(usuario).State = EntityState.Modified;


                //_db.SaveChangesAsync();

                //userResponse.Password = usuario.Password;
                //userResponse.Email = usuario.Email;
                //userResponse.Token = GetToken(usuario);

                //Se asigna el usuario 

                var empresa = _db.Empresas.Where(d => d.EmpresaId == usuario.EmpresaId).FirstOrDefault();

                usuario.Empresa = empresa;

                usuarioM = usuario;


            }
            //return userResponse;
            //Se recupera el usuario
            respuesta.Exito = 1;
            respuesta.Data = usuarioM;
            return respuesta;
        }

        //Se valida la contraseña y usario para el login
        public Usuario AuthSaml(string email)
        {
            //UserResponse userResponse = new UserResponse();
            //Se crea una variable del tipo de servicio para poder cifrar información
            CifradoHelper cifradoHelper = new CifradoHelper();

            //Se crea la variable para recuperar el usuario
            Usuario usuarioM = new Usuario();
            using (var db = _db)
            {
                //Se recupera el usuario con respecto al email
                var usuario = _db.Usuarios.Where(d => d.EmailSSO == email && d.UsuarioEstatusSesion == false).FirstOrDefault();

                //Si no existe el usuario retorna un null
                if (usuario == null)
                {
                    return null;
                }

                ////Si existe el usuario se comparan las contraseñas decifradas
                //if (cifradoHelper.DecryptStringAES(usuario.Password) != sPassword)
                //{
                //    return null;
                //}



                //Se crea y se recupera el token de seguridad 
                usuario.UsuarioToken = GetToken(usuario);
                //_db.Entry(usuario).State = EntityState.Modified;


                //_db.SaveChangesAsync();

                //userResponse.Password = usuario.Password;
                //userResponse.Email = usuario.Email;
                //userResponse.Token = GetToken(usuario);

                //Se asigna el usuario 

                //var empresa = _db.Empresas.Where(d => d.EmpresaId == usuario.EmpresaId).FirstOrDefault();

                //usuario.Empresa = empresa;
                usuarioM.UsuarioId = usuario.UsuarioId;
                usuarioM.Email = usuario.Email;
                usuarioM.UsuarioFechaLimite = DateTime.Now.AddMinutes(10);
                //usuarioM.UsuarioClave = usuario.UsuarioClave;
                usuarioM.UsuarioToken = usuario.UsuarioToken;
                usuarioM.ImagePath = usuario.ImagePath;
                usuarioM.RolId = usuario.RolId;
                usuarioM.EmpleadoNoEmp = usuario.EmpleadoNoEmp;
                usuarioM.EmpresaId = usuario.EmpresaId;


            }
            //return userResponse;
            //Se recupera el usuario
            return usuarioM;
        }

        private string GetToken(Usuario usuario)
        {
            //Se utiliza Jwt para crear un token de seguridad
            var tokenHandler = new JwtSecurityTokenHandler();

            //Se recupera la llave de cifrado
            var llave = Encoding.ASCII.GetBytes(_appSettings.Secreto);

            //Se crea el token con respecto al id del usaurio y su correo
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email)
                    }
                    ),
                //Se asigna 1 dia de vigencia para el token
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256Signature)
            };

            //Se crea el token y se recupera
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
