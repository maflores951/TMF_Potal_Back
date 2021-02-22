using LoginBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fintech.API.Helpers
{
    public class ParametroHelper
    {

        private readonly DataContext _db;

        public ParametroHelper(DataContext db){
            _db = db;
            }

        public static async System.Threading.Tasks.Task<Parametro> RecuperaParametro(string parametroClave, DataContext db)
        {
            //var db = new DataContext();

            //Se busca la información del parametro en la tabla Parametros por medio de la clave
            var parametro = await db.Parametros.
               Where(p => p.ParametroClave.ToLower() == parametroClave.ToLower()).
               FirstOrDefaultAsync();

            //Se valida si existe el parametro
            if (parametro == null)
            {
                return null;
            }

            return parametro;
        }
    }
}