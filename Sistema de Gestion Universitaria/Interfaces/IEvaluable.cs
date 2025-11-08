using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Gestion_Universitaria.Interfaces
{
    public interface IEvaluable
    {
        void AgregarCalificacion(decimal calificacion);
        decimal ObtnerPromedio();
        bool HaAprobado();
    }
}
