using LoginBase.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Services
{
    public class ComparativoEspecial : IComparativoEspecial
    {
        //public bool CompararCUOTAS_OP_RCV(double valorTemM, double valorSua, double valorEma)
        //{
        //    Respuesta respuesta = new Respuesta();
        //    var estatusComparacion = false;
        //        if (valorTemM == valorSua || (valorTemM < valorSua + 0.05 && valorTemM > valorSua - 0.05))
        //        {
        //                if (valorTemM == valorEma || (valorTemM < valorEma + 0.05 && valorTemM > valorEma - 0.05))
        //                {
        //            estatusComparacion = true;
        //                }
        //                else
        //                {
        //            estatusComparacion = false;
        //                }
        //            }
        //    else
        //    {
        //        estatusComparacion = false;
        //    }
        //    return estatusComparacion;
        //}

        public bool CR_INFONAVIT(double valorTemM, double valorSua, double valorEma)
        {
            Respuesta respuesta = new Respuesta();
            var estatusComparacion = false;
            if (valorTemM == valorSua || (valorTemM < valorSua + 1 && valorTemM > valorSua - 1))
            {
                if (valorTemM == valorEma || (valorTemM < valorEma + 1 && valorTemM > valorEma - 1))
                {
                    estatusComparacion = true;
                }
                else
                {
                    estatusComparacion = false;
                }
            }
            else
            {
                estatusComparacion = false;
            }
            return estatusComparacion;
        }

        public bool CompararCUOTAS_OP_RCV(double valorTemM, double valorSua, double valorEma)
        {
            Respuesta respuesta = new Respuesta();
            var estatusComparacion = false;
            if (valorTemM == valorSua || (valorTemM < valorSua + 0.05 && valorTemM > valorSua - 0.05))
            {
                if (valorTemM == valorEma || (valorTemM < valorEma + 0.05 && valorTemM > valorEma - 0.05))
                {
                    estatusComparacion = true;
                }
                else
                {
                    estatusComparacion = false;
                }
            }
            else
            {
                estatusComparacion = false;
            }
            return estatusComparacion;
        }
    }
}
