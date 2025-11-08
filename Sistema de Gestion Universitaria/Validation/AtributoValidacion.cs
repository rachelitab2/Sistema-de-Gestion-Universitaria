using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_Universitaria.Validation
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AtributoValidacion : Attribute
    {
        public string Mensaje { get; }
        public AtributoValidacion(string mensaje = null)
        {

            Mensaje = mensaje;
        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public class ValidacionRangoAttribute : Attribute
        {
            public decimal Minimo { get; }
            public decimal Maximo { get; }

            public ValidacionRangoAttribute(double min, double max)
            {
                Minimo = (decimal)min;
                Maximo = (decimal)max;
            }

        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public class FormatoAttribute : Attribute
        {
            public string Patron { get; }
            public string Mensaje { get; }

            public FormatoAttribute(string patron, string mensaje = null)
            {
                Patron = patron;
                Mensaje = mensaje;

            }
        }
    }
}
