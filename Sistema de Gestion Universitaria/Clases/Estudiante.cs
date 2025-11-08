using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_de_Gestion_Universitaria.Interfaces;

namespace Sistema_de_Gestion_Universitaria.Clases
{
    public class Estudiante : Persona, IIdentificable
    {
        public string carrera { get; set; }
        public string matricula { get; set; }


        public string _Carrera
        {
            get => carrera;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Carrera no puede estar vacio");
                carrera = value.Trim();
            }
        }

        public string _Matricula
        {
            get => matricula;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Matricula no puede estar vacio");
                matricula = value.Trim();
            }
        }

        public Estudiante()
        {

        }

        public Estudiante(string identificacion, string nombre, string apellido, DateTime fechaNacimiento, string carrera, string matricula)
        {
            _indentificacion = identificacion;
            _nombre = nombre;
            _apellido = apellido;
            _fechaNacimiento = fechaNacimiento;
            _Carrera = carrera;
            _Matricula = matricula;

            ValidarEdadMinima(15);
        }

        public override string ObtenerRol() => "Estudiante";
    }
}
