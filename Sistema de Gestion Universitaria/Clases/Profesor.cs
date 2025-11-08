using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Gestion_Universitaria.Interfaces;

namespace Sistema_Gestion_Universitaria.Clases
{
    public enum TipoContrato
    {
        TiempoCompleto, 
        MedioTiempo, 
        Adjunto
    }
    public class Profesor : Persona, IIdentificable
    {
        public string Departamento { get; set; }
        public TipoContrato TipoContrato { get; set; }
        public decimal SalarioBase { get; set; }

        public string _Departamento
        {
            get => Departamento;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException("El campo de Departamento no puede estar vacio");
                Departamento = value.Trim();
            }
        }


        public decimal _SalarioBase
        {
            get => SalarioBase;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("El salario base no puede ser negativo");
                SalarioBase = value;
            }
        }

        public Profesor()
        {

        }

        public Profesor(string identificacion, string nombre, string apellido, DateTime fechaNacimiento, string departamento, TipoContrato tipoContrato, decimal salarioBase)
        {
            _identificacion = identificacion;
            _nombre = nombre;
            _apellido = apellido;
            _fechaNacimiento = fechaNacimiento;
            _Departamento = departamento;
            TipoContrato = tipoContrato;
            _SalarioBase = salarioBase;

            ValidarEdadMinima(25);
        }

        public override string ObtenerRol() => "Profesor";

    }
}
