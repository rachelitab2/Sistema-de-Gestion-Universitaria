using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Gestion_Universitaria.Interfaces;

namespace Sistema_Gestion_Universitaria.Clases
{
    public class Estudiante : Persona, IIdentificable
    {
        public string Carrera { get; set; }
        public string NumeroMatricula { get; set; } 


        public string _Carrera
        {
            get => Carrera;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Carrera no puede estar vacio");
                Carrera = value.Trim();
            }
        }

        public string _Matricula
        {
            get => NumeroMatricula;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Matricula no puede estar vacio");
                NumeroMatricula = value.Trim();
            }
        }

        public Estudiante()
        {

        }

        public Estudiante(string identificacion, string nombre, string apellido, DateTime fechaNacimiento, string carrera, string matricula)
        {
            _identificacion = identificacion;
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
