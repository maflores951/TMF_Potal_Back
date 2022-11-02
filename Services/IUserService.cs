using LoginBase.Models;
using LoginBase.Models.Request;
using LoginBase.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Services
{
    public interface IUserService
    {
        Respuesta Auth(AuthRequest model);

        Usuario AuthSaml(string email);
    }
}
