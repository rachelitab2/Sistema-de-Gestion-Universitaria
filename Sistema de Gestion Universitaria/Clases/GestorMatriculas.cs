using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Reflection;

namespace Sistema_de_Gestion_Universitaria.Clases
{
    public class GestorMatriculas
    {
        private readonly List<Matricula> _matriculas = new List<Matricula>();

        private readonly Repositorio<Estudiante> _repoEstudiantes;
        private readonly Repositorio<Profesor> _repoProfesores;
        private readonly Repositorio<Curso> _repoCursos;

        public GestorMatriculas(Repositorio<Estudiante> repoEstudiantes,
            Repositorio<Profesor> repoProfesores,
            Repositorio<Curso> repoCursos)
        {
            _repoEstudiantes = repoEstudiantes ?? throw new ArgumentNullException(nameof(repoEstudiantes));
            _repoProfesores = repoProfesores ?? throw new ArgumentNullException(nameof(repoProfesores));
            _repoCursos = repoCursos ?? throw new ArgumentNullException(nameof(repoCursos));
        }

        private static string Key(string idEstudiante, string codigoCurso)
        {
            return $"{idEstudiante?.Trim().ToUpper()}|{codigoCurso?.Trim().ToUpper()}";
        }
        public void MaricularEstudiante(Estudiante estudiante, Curso curso)
        {
            if (estudiante == null) throw new ArgumentNullException(nameof(estudiante));
            if (curso == null) throw new ArgumentNullException(nameof(curso));

            if (_repoEstudiantes.BuscarPorId(estudiante.Identificacion) == null)
                throw new InvalidOperationException("El estudiante no esta registrado en el sistema");

            if (_repoCursos.BuscarPorId(curso.Codigo) == null)
                throw new InvalidOperationException("El curso no esta registrado en el sistema");

            var existe = _matriculas.Any(m =>
                m.Estudiante.Identificacion.Equals(estudiante.Identificacion, StringComparison.OrdinalIgnoreCase) &&
                m.Curso.Codigo.Equals(curso.Codigo, StringComparison.OrdinalIgnoreCase));

            if (existe)
                throw new InvalidOperationException("El estudiante ya esta matriculado en ese curso");

            var nueva = new Matricula(estudiante, curso, DateTime.Now);
            _matriculas.Add(nueva);
        }

        public void AgregarCalificacion(string idEstudiante, string codigoCurso, decimal calificacion)
        {
            if (string.IsNullOrWhiteSpace(idEstudiante))
                throw new ArgumentException("El id estudiante vacio");
            if (string.IsNullOrWhiteSpace(codigoCurso))
                throw new ArgumentException("Codigo curso vacio");

            if (calificacion < 0m || calificacion > 10m)
                throw new ArgumentOutOfRangeException("La calificacion debe estar entre 0 y 10");

            var mat = _matriculas.FirstOrDefault(m =>
            m.Estudiante.Identificacion.Equals(idEstudiante, StringComparison.OrdinalIgnoreCase) &&
                m.Curso.Codigo.Equals(codigoCurso, StringComparison.OrdinalIgnoreCase));

            if (mat == null)
                throw new KeyNotFoundException("No existe una matricula para ese estudiante y curso");

            mat.AgregarCalificacion(calificacion);

        }



        public List<Matricula> ObtenerMatriculasPorEstudiante(string idEstudiante)
        {
            if (string.IsNullOrWhiteSpace(idEstudiante)) return new List<Matricula>();
            return _matriculas
                .Where(m => m.Estudiante.Identificacion.Equals(idEstudiante, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Estudiante> ObtenerMatriculasPorCurso(string codigoCurso)
        {
            if (string.IsNullOrWhiteSpace(codigoCurso)) return new List<Estudiante>();

            return _matriculas
                .Where(m => m.Curso.Codigo.Equals(codigoCurso, StringComparison.OrdinalIgnoreCase))
                .Select(m => m.Estudiante)
                .Distinct()
                .ToList();


        }
        public string GenerarReporteMatriculas(string idEstudiante)
        {
            var mats = ObtenerMatriculasPorEstudiante(idEstudiante);
            if (mats.Count == 0)
                return "No hay matriculas para este estudiante.";

            var est = mats.First().Estudiante;
            var lineas = new List<string>
            {
                $"Reporte de: {est.Nombre} {est.Apellido} (ID: {est.Identificacion})"
            };

            foreach (var m in mats.OrderBy(m => m.Curso.Nombre))
            {
                lineas.Add($"- Curso: {m.Curso.Nombre} | Estado: {m.ObtenerEstado()} | Promedio: {m.ObtenerPromedio():0.00}");
            }

            var promedios = mats.Select(mm => mm.Calificaciones.Count > 0 ? mm.ObtenerPromedio() : (decimal?)null)
                  .Where(v => v.HasValue)
                  .Select(v => v!.Value)
                  .ToList();

            if (promedios.Count > 0)
            {
                var promedioGeneral = promedios.Average();
                lineas.Add($"Promedio General: {promedioGeneral:0.00}");
            }
            else
            {
                lineas.Add("Promedio General: N/A (sin calificaciones) ");
            }

            return string.Join(Environment.NewLine, lineas);


        }

        public List<Matricula> ObtenerTodasLasMatriculas() => _matriculas.ToList();

        private decimal PromedioGeneralDeEstudiante(string idEst)
        {
            var mats = _matriculas
                .Where(m => m.Estudiante.Identificacion.Equals(idEst, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var promedios = mats
                .Select(m => m.Calificaciones.Count > 0 ? m.ObtenerPromedio() : (decimal?)null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            return promedios.Count == 0 ? 0m : promedios.Average();

        }

        public List<(Estudiante Estudiante, decimal Promedio)> ObtenerTop10Estudiantes()
        {
            var query =
                _matriculas
                .GroupBy(m => m.Estudiante.Identificacion)
                .Select(g => new
                {
                    Id = g.Key,
                    Est = g.First().Estudiante,
                    Promedio = g
                    .Select(mm => mm.Calificaciones.Count > 0 ? mm.ObtenerPromedio() : (decimal?)null)
                    .Where(v => v.HasValue)
                    .Select(v => v!.Value)
                    .DefaultIfEmpty(0m)
                    .Average()
                })
                .OrderByDescending(x => x.Promedio)
                .ThenBy(x => x.Est.Apellido)
                .ThenBy(x => x.Est.Nombre)
                .Take(10)
                .Select(x => (x.Est, x.Promedio))
                .ToList();

            return query;

        }

        public List<(Estudiante Estudiante, decimal Promedio)> ObtenerEstudiantesEnRiesgo()
        {
            var query =
                _matriculas
                .GroupBy(m => m.Estudiante.Identificacion)
                .Select(g => new
                {
                    Est = g.First().Estudiante,
                    Promedio = g
                        .Select(mm => mm.Calificaciones.Count > 0 ? mm.ObtenerPromedio() : (decimal?)null)
                        .Where(v => v.HasValue)
                        .Select(v => v!.Value)
                        .DefaultIfEmpty(0m)
                        .Average()
                })
                .Where(x => x.Promedio < 7.0m)
                .OrderBy(x => x.Promedio)
                .Select(x => (x.Est, x.Promedio))
                .ToList();

            return query;
        }

        public List<(Curso Curso, int CantidadEstudiantes)> ObtenerCursosMasPopulares()
        {
            var query =
                _matriculas
                .GroupBy(m => m.Curso.Codigo)
                .Select(g => new
                {
                    Curso = g.First().Curso,
                    Cantidad = g
                        .Select(m => m.Estudiante.Identificacion)
                        .Distinct()
                        .Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ThenBy(x => x.Curso.Nombre)
                .Select(x => (x.Curso, x.Cantidad))
                .ToList();

            return query;
        }

        public decimal ObtenerPromedioGeneral()
        {
            var promediosPorMatricula =
                _matriculas
                .Select(m => m.Calificaciones.Count > 0 ? (decimal?)m.ObtenerPromedio() : null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            return promediosPorMatricula.Count == 0 ? 0m : promediosPorMatricula.Average();
        }

        public List<(string Carrera, int Cantidad, decimal PromedioGeneral)> ObtenerEstadisticasPorCarrera()
        {
            var query =
                _matriculas
                .GroupBy(m => ObtenerCarrera(m.Estudiante))     
                .Select(g =>
                {
                    var estudiantesUnicos = g.Select(m => m.Estudiante).Distinct().ToList();

                    var promediosDeEstudiantes = estudiantesUnicos
                        .Select(est =>
                        {
                            var matsEst = _matriculas.Where(m => m.Estudiante.Identificacion == est.Identificacion);
                            var proms = matsEst
                                .Select(mm => mm.Calificaciones.Count > 0 ? mm.ObtenerPromedio() : (decimal?)null)
                                .Where(v => v.HasValue)
                                .Select(v => v!.Value)
                                .ToList();
                            return proms.Count == 0 ? 0m : proms.Average();
                        })
                        .ToList();

                    var promedioCarrera = promediosDeEstudiantes.Count == 0 ? 0m : promediosDeEstudiantes.Average();

                    return (Carrera: g.Key, Cantidad: estudiantesUnicos.Count, PromedioGeneral: promedioCarrera);
                })
                .OrderByDescending(x => x.PromedioGeneral)
                .ThenByDescending(x => x.Cantidad)
                .ToList();

            return query;
        }

        public List<Estudiante> BuscarEstudiantes(Func<Estudiante, bool> criterio)
        {
            if (criterio == null) return new List<Estudiante>();

            var estudiantes =
                _matriculas
                .Select(m => m.Estudiante)
                .Distinct()
                .Where(criterio)
                .ToList();

            return estudiantes;
        }

        public List<Estudiante> ObtenerAprobadosPorCurso(string codigoCurso)
        {
            if (string.IsNullOrWhiteSpace(codigoCurso)) return new List<Estudiante>();

            return _matriculas
                .Where(m => m.Curso.Codigo.Equals(codigoCurso, StringComparison.OrdinalIgnoreCase))
                .Where(m => m.Calificaciones.Count > 0 && m.ObtenerPromedio() >= 7m) // lambda extra
                .Select(m => m.Estudiante)
                .Distinct()
                .OrderBy(e => e.Apellido) // lambda extra
                .ThenBy(e => e.Nombre)
                .ToList();
        }

        public List<(Estudiante Estudiante, decimal Promedio)> RankingPorCurso(string codigoCurso, int top = 10)
        {
            if (string.IsNullOrWhiteSpace(codigoCurso)) return new List<(Estudiante, decimal)>();

            return _matriculas
                .Where(m => m.Curso.Codigo.Equals(codigoCurso, StringComparison.OrdinalIgnoreCase))
                .Select(m => (m.Estudiante, Prom: m.Calificaciones.Count > 0 ? m.ObtenerPromedio() : 0m))
                .OrderByDescending(x => x.Prom) // lambda extra
                .ThenBy(x => x.Estudiante.Apellido)
                .ThenBy(x => x.Estudiante.Nombre)
                .Take(top)
                .ToList();
        }

        public List<Estudiante> BuscarEstudiantesPorTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return new List<Estudiante>();
            texto = texto.Trim().ToUpperInvariant();

            return _matriculas
                .Select(m => m.Estudiante)
                .Distinct()
                .Where(e => ($"{e.Nombre} {e.Apellido}").ToUpperInvariant().Contains(texto))
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToList();
        }

        // Helper: intenta obtener el valor de "carrera" mediante reflexión para evitar dependencia directa en la propiedad.
        private static string ObtenerCarrera(Estudiante est)
        {
            if (est == null) return "Sin Carrera";

            // Buscar propiedades que podrían contener la "carrera" (insensible a mayúsculas)
            var posiblesNombres = new[] { "Carrera", "Programa", "Facultad", "Major" };
            foreach (var nombre in posiblesNombres)
            {
                var prop = est.GetType().GetProperty(nombre, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    var val = prop.GetValue(est) as string;
                    if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
                }
            }

            // Si no se encuentra, retornar un valor por defecto
            return "Sin Carrera";
        }

    }
}
