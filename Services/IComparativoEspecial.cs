using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginBase.Services
{
    public interface IComparativoEspecial
    {
        bool CompararCUOTAS_OP_RCV(double valorTemM, double valorSua, double valorEma);

        bool CR_INFONAVIT(double valorTemM, double valorSua, double valorEma);
    }
}
