using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_de_Gestion_Universitaria.Interfaces;

namespace Sistema_de_Gestion_Universitaria.Clases
{
    public class Curso : IIdentificable
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        public int Credito { get; set; }

        public string _Codigo
        {
            get => Codigo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Codigo no puede estar vacio");
                Codigo = value.Trim();
            }
        }

        public string _Nombre
        {
            get => Nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Nombre no puede estar vacio");
                Nombre = value.Trim();
            }
        }

        public int _Credito
        {
            get => Credito;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("El credito debe ser un numero positivo");
                Credito = value;
            }
        }

        public Profesor ProfesorAsignado { get; set; }

        public string Identificacion => Codigo;
        public Curso()
        {

        }

        public Curso(string codigo, string nombre, int credito, Profesor profesorAsignado)
        {
            _Codigo = codigo;
            _Nombre = nombre;
            _Credito = credito;
            ProfesorAsignado = profesorAsignado;
        }

        public override string ToString()

        => $"Curso: {Nombre} (Codigo: {Codigo}, Credito: {Credito}), Profesor Asignado: {ProfesorAsignado._nombre} {ProfesorAsignado._apellido}";

    }
}
