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

        //Comparativo con una diferencia de +-1
        public bool CR_INFONAVIT(double valorTemM, int posicionTem, double valorSua, int posicionSua, double valorEma, int posicionEma)
        {
            Respuesta respuesta = new Respuesta();
            var estatusComparacion = false;
            if (posicionTem > 1 && posicionSua > 1)
            {
                if (valorTemM == valorSua || (valorTemM <= valorSua + 1 && valorTemM >= valorSua - 1))
                {
                    if (posicionEma > 1)
                    {
                        if (valorTemM == valorEma || (valorTemM <= valorEma + 1 && valorTemM >= valorEma - 1))
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
                        estatusComparacion = true;
                    }

                }
                else
                {
                    estatusComparacion = false;
                }
            }
            else if (posicionTem > 1 && posicionEma > 1)
            {
                if (valorTemM == valorEma || (valorTemM <= valorEma + 1 && valorTemM >= valorEma - 1))
                {
                    estatusComparacion = true;
                }
                else
                {
                    estatusComparacion = false;
                }

            }
            else if (posicionSua > 1 && posicionEma > 1)
            {
                if (valorSua == valorEma || (valorSua <= valorEma + 1 && valorSua >= valorEma - 1))
                {
                    estatusComparacion = true;
                }
                else
                {
                    estatusComparacion = false;
                }
            }

            return estatusComparacion;
        }

        //Comparativo con una diferencia de +-0.05
        public bool CompararCUOTAS_OP_RCV(double valorTemM, int posicionTem, double valorSua, int posicionSua, double valorEma, int posicionEma)
        {
            Respuesta respuesta = new Respuesta();
            var estatusComparacion = false;
            if (posicionTem > 1 && posicionSua > 1)
            {
                if (valorTemM == valorSua || (valorTemM <= valorSua + 0.05 && valorTemM >= valorSua - 0.05))
                {
                    if (posicionEma > 1)
                    {
                        if (valorTemM == valorEma || (valorTemM <= valorEma + 0.05 && valorTemM >= valorEma - 0.05))
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
                        estatusComparacion = true;
                    }

                }
                else
                {
                    estatusComparacion = false;
                }
            }
            else if (posicionTem > 1 && posicionEma > 1)
            {
                if (valorTemM == valorEma || (valorTemM <= valorEma + 0.05 && valorTemM >= valorEma - 0.05))
                {
                    estatusComparacion = true;
                }
                else
                {
                    estatusComparacion = false;
                }

            }
            else if (posicionSua > 1 && posicionEma > 1)
            {
                if (valorSua == valorEma || (valorSua <= valorEma + 0.05 && valorSua >= valorEma - 0.05))
                {
                    estatusComparacion = true;
                }
                else
                {
                    estatusComparacion = false;
                }
            }

            return estatusComparacion;
        }

        //Comparativo con de fechas para recuperar los dias
        public bool Dias(double valorTemM, int posicionTem, double valorSua, int posicionSua, double valorEma, int posicionEma)
        {
            Respuesta respuesta = new Respuesta();
            var estatusComparacion = false;



            if (posicionTem >= 1  && posicionSua >= 1)
            {
                if (valorTemM == valorSua)
                {
                    if (posicionEma >= 1)
                    {
                        if (valorTemM == valorEma)
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

            //}
            //    else
            //    {
            //        estatusComparacion = false;
            //    }
            //}
            //else if (posicionTem > 1 && posicionEma > 1)
            //{
            //    if (valorTemM == valorEma)
            //    {
            //        estatusComparacion = true;
            //    }
            //    else
            //    {
            //        estatusComparacion = false;
            //    }

            //}
            //else if (posicionSua > 1 && posicionEma > 1)
            //{
            //    if (valorSua == valorEma)
            //    {
            //        estatusComparacion = true;
            //    }
            //    else
            //    {
            //        estatusComparacion = false;
            //    }
            //}

            return estatusComparacion;
        }
    }
}
