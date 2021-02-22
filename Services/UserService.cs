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

        public Usuario Auth(AuthRequest model)
        {
            //UserResponse userResponse = new UserResponse();
            CifradoHelper cifradoHelper = new CifradoHelper();

            Usuario usuarioM = new Usuario();
            using (var db = _db)
            {
                string sPassword = cifradoHelper.DecryptStringAES(model.Password);

                //var usuario = db.Usuarios.FirstOrDefault();
                //var usuario = _db.Usuarios.Where(d => d.Email == model.Email && cifradoHelper.DecryptStringAES(d.Password) == sPassword).FirstOrDefault();

                var usuario = _db.Usuarios.Where(d => d.Email == model.Email).FirstOrDefault();

                if (usuario == null)
                {
                    return null;
                }

                if (cifradoHelper.DecryptStringAES(usuario.Password) != sPassword)
                {
                    return null;
                }

                //usuario.UsuarioToken = GetToken(usuario);
                //_db.Entry(usuario).State = EntityState.Modified;


                //_db.SaveChangesAsync();

                //userResponse.Password = usuario.Password;
                //userResponse.Email = usuario.Email;
                //userResponse.Token = GetToken(usuario);

                usuarioM = usuario;


            }
            //return userResponse;
            return usuarioM;
        }

        private string GetToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var llave = Encoding.ASCII.GetBytes(_appSettings.Secreto);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                        new Claim(ClaimTypes.Email, usuario.Email)
                    }
                    ),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
