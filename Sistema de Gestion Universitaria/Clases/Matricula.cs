using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Gestion_Universitaria.Interfaces;

namespace Sistema_Gestion_Universitaria.Clases
{
    public class Matricula : IEvaluable
    {
        public Estudiante Estudiante { get; set; }
        public Curso Curso { get; set; }
        public DateTime FechaMatricula { get; set; }
        public List<decimal> Calificaciones { get; } = new List<decimal>();

        public Matricula()
        {

        }

        public Matricula(Estudiante estudiante, Curso curso, DateTime fechaMatricula)
        {
            Estudiante = estudiante ?? throw new ArgumentException(nameof(estudiante));
            Curso = curso ?? throw new ArgumentException(nameof(curso));
            FechaMatricula = fechaMatricula;
        }

        public void AgregarCalificacion(decimal calificacion)
        {
            if (calificacion < 0m || calificacion > 10m)
                throw new ArgumentOutOfRangeException("La calificacion debe estar entre 0 y 10");
            Calificaciones.Add(calificacion);
        }

        public decimal ObtenerPromedio() // Versión ortográfica correcta
        {
            if (Calificaciones.Count == 0) return 0m;
            return Calificaciones.Average();
        }

        // Implementación para cumplir la interfaz existente que tiene la falta de ortografía.
        // Delegamos a la versión correcta para evitar duplicación.
        public decimal ObtnerPromedio()
        {
            return ObtenerPromedio();
        }

        public bool HaAprobado()
        {
            return ObtenerPromedio() >= 7.0m;
        }

        public string ObtenerEstado()
        {
            if (Calificaciones.Count == 0) return "En curso";
            return HaAprobado() ? "Aprobado" : "Reprobado";
        }

        public override string ToString()
        {
            return $"Matricula: {Estudiante?.Nombre} {Estudiante?.Apellido} en {Curso?.Nombre} - Estado: {ObtenerEstado()} - Promedio: {ObtenerPromedio():0.00}";
        }
    }
}
