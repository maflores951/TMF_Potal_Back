using Fintech.API.Helpers;
using LoginBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tmf_group.Helper
{
    public static class CifrarUsuario
    {
        public static string GeneraUsuario(Usuario usuarioLogin)
        {

            CifradoHelper cifradoHelper = new CifradoHelper();
            string emailCifrado;
            try
            {
                emailCifrado = cifradoHelper.EncryptStringAES(Newtonsoft.Json.JsonConvert.SerializeObject(usuarioLogin));
            }
            catch (Exception)
            {
                return null;
            }
            return emailCifrado;
        }
    }
}
