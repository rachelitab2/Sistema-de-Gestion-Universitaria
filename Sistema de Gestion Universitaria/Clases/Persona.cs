using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sistema_de_Gestion_Universitaria.Clases
{
    public abstract class Persona
    {
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }


        public string _indentificacion
        {
            get => Identificacion;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("La Identificacion no puede estar vacia");
                Identificacion = value;
            }


        }

        public string _nombre
        {
            get => Nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Nombre no peude estar vacio");
                Nombre = value;
            }
        }

        public string _apellido
        {
            get => Apellido;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("Por favor complete el Campo de Apellido");
                Apellido = value;
            }
        }

        public DateTime _fechaNacimiento
        {
            get => FechaNacimiento;
            set
            {
                if (value.Date > DateTime.Today)
                    throw new ArgumentOutOfRangeException("La fecha de nacimiento no puede ser mayor o igual a la fecha actual");
                FechaNacimiento = value.Date;

            }
        }
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }
        protected void ValidarEdadMinima(int edadMinima)
        {
            if (Edad < edadMinima)
                throw new ArgumentException($"La edad minima es de {edadMinima} años.");
        }
        public abstract string ObtenerRol();
        public override string ToString()
        {
            return $"{ObtenerRol()}: {Nombre} {Apellido}, ID: {Identificacion}, Edad: {Edad} años";
        }
    }
}
