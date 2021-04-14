using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Services
{
    public interface IComparativoEspecial
    {
        bool CompararCUOTAS_OP_RCV(double valorTemM, int posicionTem, double valorSua, int posicionSua, double valorEma, int posicionEma);

        bool CR_INFONAVIT(double valorTemM, int posicionTem, double valorSua, int posicionSua, double valorEma, int posicionEma);
    }
}
