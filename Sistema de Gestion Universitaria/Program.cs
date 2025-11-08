using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Sistema_Gestion_Universitaria.Validation;

namespace Sistema_Gestion_Universitaria.Clases
{
    internal class Program
    {
        static Repositorio<Estudiante> repoEstudiantes = new Repositorio<Estudiante>();
        static Repositorio<Profesor> repoProfesores = new Repositorio<Profesor>();
        static Repositorio<Curso> repoCursos = new Repositorio<Curso>();
        static GestorMatriculas gestor = new GestorMatriculas(repoEstudiantes, repoProfesores, repoCursos);



        static void Main(string[] args)
        {
            Console.Title = "Sistema de Gestion Universitaria";
        
           

            MenuPrincipal();
        }

        static void SetInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void SetOk(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void SetWarn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static void SetError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        static string ReadNonEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s))
                    return s.Trim();
                SetWarn("Valor requerido. Intenta de nuevo.");
            }
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out var n) && n >= min && n <= max)
                    return n;
                SetWarn($"Ingresa un entero entre {min} y {max}.");
            }
        }

        static decimal ReadDecimal(string prompt, decimal min, decimal max, bool usarFormatoCalificacion = false)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (usarFormatoCalificacion)
                {
                    if (Utilidades.ParsearCalificacion(s, out var cal))
                        return cal;
                    SetWarn("Calificación inválida. Usa número 0..10 (ej: 8.5).");
                }
                else
                {
                    if (decimal.TryParse(s, out var d) && d >= min && d <= max)
                        return d;
                    SetWarn($"Ingresa un decimal entre {min} y {max}.");
                }
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (DateTime.TryParse(s, out var dt) && dt.Date <= DateTime.Today)
                    return dt.Date;
                SetWarn("Fecha inválida. Usa formato válido (ej: 2004-05-10) y no futura.");
            }
        }

        //  Menu Principal
        static void MenuPrincipal()
        {
            while (true)
            {
                Console.Clear();
                SetInfo("=== Sistema de Gestión Universitaria ===");
                Console.WriteLine("1) Gestionar Estudiantes");
                Console.WriteLine("2) Gestionar Profesores");
                Console.WriteLine("3) Gestionar Cursos");
                Console.WriteLine("4) Matricular Estudiante en Curso");
                Console.WriteLine("5) Registrar Calificaciones");
                Console.WriteLine("6) Ver Reportes y Estadísticas");
                Console.WriteLine("7) Análisis con Reflection");
                Console.WriteLine("0) Salir");

                var op = ReadInt("Elige opción: ", 0, 7);
                try
                {
                    switch (op)
                    {
                        case 1: MenuEstudiantes(); break;
                        case 2: MenuProfesores(); break;
                        case 3: MenuCursos(); break;
                        case 4: AccionMatricular(); break;
                        case 5: AccionRegistrarCalificacion(); break;
                        case 6: MenuReportes(); break;
                        case 7: AccionReflection(); break;
                        case 0: return;
                    }
                }
                catch (Exception ex)
                {
                    SetError($"Error: {ex.Message}");
                    Console.WriteLine("Presiona una tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        // ==== Submenú Estudiantes ====
        static void MenuEstudiantes()
        {
            while (true)
            {
                Console.Clear();
                SetInfo("=== Estudiantes ===");
                Console.WriteLine("1) Agregar");
                Console.WriteLine("2) Listar");
                Console.WriteLine("3) Buscar por ID");
                Console.WriteLine("4) Modificar");
                Console.WriteLine("5) Eliminar");
                Console.WriteLine("0) Volver");
                var op = ReadInt("Opción: ", 0, 5);

                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: EstudianteAgregar(); break;
                        case 2: EstudianteListar(); break;
                        case 3: EstudianteBuscar(); break;
                        case 4: EstudianteModificar(); break;
                        case 5: EstudianteEliminar(); break;
                    }
                }
                catch (Exception ex)
                {
                    SetError($"Error: {ex.Message}");
                }

                Console.WriteLine("Presiona una tecla para continuar...");
                Console.ReadKey();
            }
        }

        static void EstudianteAgregar()
        {
            Console.Clear();
            SetInfo("Agregar Estudiante");
            var id = ReadNonEmpty("Identificación: ");
            var nombre = ReadNonEmpty("Nombre: ");
            var apellido = ReadNonEmpty("Apellido: ");
            var fecha = ReadDate("Fecha nacimiento (YYYY-MM-DD): ");
            var carrera = ReadNonEmpty("Carrera: ");
            var matricula = ReadNonEmpty("Matrícula (YYYY-NNNN): ");

            var est = new Estudiante
            {
                Identificacion = id,
                Nombre = nombre,
                Apellido = apellido,
                FechaNacimiento = fecha,
                Carrera = carrera,
                NumeroMatricula = matricula
            };

            var errores = Validador.Validar(est);
            if (errores.Any())
            {
                SetWarn("No se puede agregar. Errores:");
                foreach (var e in errores) Console.WriteLine("- " + e);
                return;
            }

            repoEstudiantes.Agregar(est);
            SetOk("Estudiante agregado.");
        }

        static void EstudianteListar()
        {
            Console.Clear();
            SetInfo("Listado de Estudiantes");
            var todos = repoEstudiantes.ObtenerTodos();
            if (todos.Count == 0) { SetWarn("No hay estudiantes."); return; }
            foreach (var e in todos)
                Console.WriteLine(e.ToString());
        }

        static void EstudianteBuscar()
        {
            Console.Clear();
            var id = ReadNonEmpty("ID del estudiante: ");
            var e = repoEstudiantes.BuscarPorId(id);
            if (e == null) { SetWarn("No encontrado."); return; }
            Console.WriteLine(e.ToString());
        }

        static void EstudianteModificar()
        {
            Console.Clear();
            var id = ReadNonEmpty("ID del estudiante a modificar: ");
            var e = repoEstudiantes.BuscarPorId(id);
            if (e == null) { SetWarn("No encontrado."); return; }

            SetInfo("Deja vacío para mantener el valor actual.");
            Console.Write($"Nombre ({e.Nombre}): ");
            var nombre = Console.ReadLine();
            Console.Write($"Apellido ({e.Apellido}): ");
            var apellido = Console.ReadLine();
            Console.Write($"Carrera ({e.Carrera}): ");
            var carrera = Console.ReadLine();
            Console.Write($"Matrícula ({e.NumeroMatricula}): ");
            var mat = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nombre)) e.Nombre = nombre.Trim();
            if (!string.IsNullOrWhiteSpace(apellido)) e.Apellido = apellido.Trim();
            if (!string.IsNullOrWhiteSpace(carrera)) e.Carrera = carrera.Trim();
            if (!string.IsNullOrWhiteSpace(mat)) e.NumeroMatricula = mat.Trim();

            var errores = Validador.Validar(e);
            if (errores.Any())
            {
                SetWarn("Cambios inválidos:");
                foreach (var err in errores) Console.WriteLine("- " + err);
                return;
            }

            SetOk("Estudiante modificado.");
        }

        static void EstudianteEliminar()
        {
            Console.Clear();
            var id = ReadNonEmpty("ID del estudiante a eliminar: ");
            repoEstudiantes.Eliminar(id);
            SetOk("Estudiante eliminado.");
        }

        // ==== Acciones de matrícula y calificaciones ====
        static void AccionMatricular()
        {
            Console.Clear();
            SetInfo("Matricular Estudiante en Curso");

            var idEst = ReadNonEmpty("ID Estudiante: ");
            var est = repoEstudiantes.BuscarPorId(idEst);
            if (est == null) { SetWarn("Estudiante no existe."); return; }

            var codCur = ReadNonEmpty("Código Curso: ");
            var curso = repoCursos.BuscarPorId(codCur);
            if (curso == null) { SetWarn("Curso no existe."); return; }

            gestor.MatricularEstudiante(est, curso);
            SetOk($"Matriculado {est.Nombre} en {curso.Nombre}.");
        }

        static void AccionRegistrarCalificacion()
        {
            Console.Clear();
            SetInfo("Registrar Calificación");
            var idEst = ReadNonEmpty("ID Estudiante: ");
            var codCur = ReadNonEmpty("Código Curso: ");
            var cal = ReadDecimal("Calificación (0..10): ", 0m, 10m, usarFormatoCalificacion: true);

            gestor.AgregarCalificacion(idEst, codCur, cal);
            SetOk("Calificación registrada.");
        }

        // ==== Submenú Profesores ====

        static void MenuProfesores()
        {
            while (true)
            {
                Console.Clear();
                SetInfo("=== Profesores ===");
                Console.WriteLine("1) Agregar");
                Console.WriteLine("2) Listar");
                Console.WriteLine("3) Buscar por ID");
                Console.WriteLine("4) Modificar");
                Console.WriteLine("5) Eliminar");
                Console.WriteLine("0) Volver");
                var op = ReadInt("Opción: ", 0, 5);

                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: ProfesorAgregar(); break;
                        case 2: ProfesorListar(); break;
                        case 3: ProfesorBuscar(); break;
                        case 4: ProfesorModificar(); break;
                        case 5: ProfesorEliminar(); break;
                    }
                }
                catch (Exception ex)
                {
                    SetError($"Error: {ex.Message}");
                }

                Console.WriteLine("Presiona una tecla para continuar...");
                Console.ReadKey();
            }
        }

        static void ProfesorAgregar()
        {
            Console.Clear();
            SetInfo("Agregar Profesor");
            var id = ReadNonEmpty("Identificación: ");
            var nombre = ReadNonEmpty("Nombre: ");
            var apellido = ReadNonEmpty("Apellido: ");
            var fecha = ReadDate("Fecha nacimiento (YYYY-MM-DD): ");
            var depto = ReadNonEmpty("Departamento: ");

            Console.WriteLine("Tipo de contrato: 1) TiempoCompleto  2) MedioTiempo  3) Adjunto");
            var tipoSel = ReadInt("Opción: ", 1, 3);
            var tipo = tipoSel == 1 ? TipoContrato.TiempoCompleto : tipoSel == 2 ? TipoContrato.MedioTiempo : TipoContrato.Adjunto;

            var salario = ReadDecimal("Salario base (>0): ", 1m, 1000000m);

            var prof = new Profesor
            {
                Identificacion = id,
                Nombre = nombre,
                Apellido = apellido,
                FechaNacimiento = fecha,
                Departamento = depto,
                TipoContrato = tipo,
                SalarioBase = salario
            };

            var errores = Validador.Validar(prof);
            if (errores.Any())
            {
                SetWarn("No se puede agregar. Errores:");
                foreach (var e in errores) Console.WriteLine("- " + e);
                return;
            }

            repoProfesores.Agregar(prof);
            SetOk("Profesor agregado.");
        }

        static void ProfesorListar()
        {
            Console.Clear();
            SetInfo("Listado de Profesores");
            var todos = repoProfesores.ObtenerTodos();
            if (todos.Count == 0) { SetWarn("No hay profesores."); return; }
            foreach (var p in todos) Console.WriteLine(p.ToString());
        }

        static void ProfesorBuscar()
        {
            Console.Clear();
            var id = ReadNonEmpty("ID del profesor: ");
            var p = repoProfesores.BuscarPorId(id);
            if (p == null) { SetWarn("No encontrado."); return; }
            Console.WriteLine(p.ToString());
        }

        static void ProfesorModificar()
        {
            Console.Clear();
            var id = ReadNonEmpty("ID del profesor a modificar: ");
            var p = repoProfesores.BuscarPorId(id);
            if (p == null) { SetWarn("No encontrado."); return; }

            SetInfo("Deja vacío para mantener el valor actual.");
            Console.Write($"Nombre ({p.Nombre}): ");
            var nombre = Console.ReadLine();
            Console.Write($"Apellido ({p.Apellido}): ");
            var apellido = Console.ReadLine();
            Console.Write($"Departamento ({p.Departamento}): ");
            var depto = Console.ReadLine();
            Console.WriteLine($"TipoContrato actual: {p.TipoContrato}. Cambiar? 1) TiempoCompleto  2) MedioTiempo  3) Adjunto  0) Mantener");
            var tipoSel = ReadInt("Opción: ", 0, 3);
            Console.Write($"SalarioBase ({p.SalarioBase}): ");
            var s = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nombre)) p.Nombre = nombre.Trim();
            if (!string.IsNullOrWhiteSpace(apellido)) p.Apellido = apellido.Trim();
            if (!string.IsNullOrWhiteSpace(depto)) p.Departamento = depto.Trim();
            if (tipoSel != 0)
                p.TipoContrato = tipoSel == 1 ? TipoContrato.TiempoCompleto : tipoSel == 2 ? TipoContrato.MedioTiempo : TipoContrato.Adjunto;
            if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, out var sal) && sal > 0) p.SalarioBase = sal;

            var errores = Validador.Validar(p);
            if (errores.Any())
            {
                SetWarn("Cambios inválidos:");
                foreach (var err in errores) Console.WriteLine("- " + err);
                return;
            }

            SetOk("Profesor modificado.");
        }

        static void ProfesorEliminar()
        {
            Console.Clear();
            var id = ReadNonEmpty("ID del profesor a eliminar: ");
            repoProfesores.Eliminar(id);
            SetOk("Profesor eliminado.");
        }

        // ==== Submenú Cursos ====
        static void MenuCursos()
        {
            while (true)
            {
                Console.Clear();
                SetInfo("=== Cursos ===");
                Console.WriteLine("1) Agregar");
                Console.WriteLine("2) Listar");
                Console.WriteLine("3) Buscar por Código");
                Console.WriteLine("4) Modificar");
                Console.WriteLine("5) Eliminar");
                Console.WriteLine("6) Asignar Profesor");
                Console.WriteLine("0) Volver");
                var op = ReadInt("Opción: ", 0, 6);

                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1: CursoAgregar(); break;
                        case 2: CursoListar(); break;
                        case 3: CursoBuscar(); break;
                        case 4: CursoModificar(); break;
                        case 5: CursoEliminar(); break;
                        case 6: CursoAsignarProfesor(); break;
                    }
                }
                catch (Exception ex)
                {
                    SetError($"Error: {ex.Message}");
                }

                Console.WriteLine("Presiona una tecla para continuar...");
                Console.ReadKey();
            }
        }

        static void CursoAgregar()
        {
            Console.Clear();
            SetInfo("Agregar Curso");
            var codigo = ReadNonEmpty("Código: ");
            var nombre = ReadNonEmpty("Nombre: ");
            var creditos = ReadInt("Créditos (1..30): ", 1, 30);

            var curso = new Curso { Codigo = codigo, Nombre = nombre, Credito = creditos };

            var errores = Validador.Validar(curso);
            if (errores.Any())
            {
                SetWarn("No se puede agregar. Errores:");
                foreach (var e in errores) Console.WriteLine("- " + e);
                return;
            }

            repoCursos.Agregar(curso);
            SetOk("Curso agregado.");
        }

        static void CursoListar()
        {
            Console.Clear();
            SetInfo("Listado de Cursos");
            var todos = repoCursos.ObtenerTodos();
            if (todos.Count == 0) { SetWarn("No hay cursos."); return; }
            foreach (var c in todos) Console.WriteLine(c.ToString());
        }

        static void CursoBuscar()
        {
            Console.Clear();
            var codigo = ReadNonEmpty("Código del curso: ");
            var c = repoCursos.BuscarPorId(codigo);
            if (c == null) { SetWarn("No encontrado."); return; }
            Console.WriteLine(c.ToString());
        }

        static void CursoModificar()
        {
            Console.Clear();
            var codigo = ReadNonEmpty("Código del curso a modificar: ");
            var c = repoCursos.BuscarPorId(codigo);
            if (c == null) { SetWarn("No encontrado."); return; }

            SetInfo("Deja vacío para mantener el valor actual.");
            Console.Write($"Nombre ({c.Nombre}): ");
            var nombre = Console.ReadLine();
            Console.Write($"Créditos ({c.Credito}): ");
            var sCred = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nombre)) c.Nombre = nombre.Trim();
            if (!string.IsNullOrWhiteSpace(sCred) && int.TryParse(sCred, out var cr) && cr >= 1 && cr <= 30) c.Credito = cr;

            var errores = Validador.Validar(c);
            if (errores.Any())
            {
                SetWarn("Cambios inválidos:");
                foreach (var err in errores) Console.WriteLine("- " + err);
                return;
            }

            SetOk("Curso modificado.");
        }

        static void CursoEliminar()
        {
            Console.Clear();
            var codigo = ReadNonEmpty("Código del curso a eliminar: ");
            repoCursos.Eliminar(codigo);
            SetOk("Curso eliminado.");
        }

        static void CursoAsignarProfesor()
        {
            Console.Clear();
            SetInfo("Asignar Profesor a Curso");
            var codigo = ReadNonEmpty("Código del curso: ");
            var c = repoCursos.BuscarPorId(codigo);
            if (c == null) { SetWarn("Curso no existe."); return; }

            var profesores = repoProfesores.ObtenerTodos();
            if (profesores.Count == 0) { SetWarn("No hay profesores para asignar."); return; }

            Console.WriteLine("Profesores disponibles:");
            foreach (var p in profesores) Console.WriteLine($"- {p.Identificacion}: {p.Nombre} {p.Apellido}");

            var idProf = ReadNonEmpty("ID del profesor a asignar: ");
            var prof = repoProfesores.BuscarPorId(idProf);
            if (prof == null) { SetWarn("Profesor no existe."); return; }

            c.ProfesorAsignado = prof;
            SetOk($"Profesor {prof.Nombre} asignado al curso {c.Nombre}.");
        }
        // ==== Reportes y estadísticas ====
        static void MenuReportes()
        {
            while (true)
            {
                Console.Clear();
                SetInfo("=== Reportes y Estadísticas ===");
                Console.WriteLine("1) Reporte por Estudiante");
                Console.WriteLine("2) Top 10 Estudiantes");
                Console.WriteLine("3) Estudiantes en Riesgo");
                Console.WriteLine("4) Cursos más Populares");
                Console.WriteLine("5) Promedio General del Sistema");
                Console.WriteLine("6) Estadísticas por Carrera");
                Console.WriteLine("0) Volver");
                var op = ReadInt("Opción: ", 0, 6);

                if (op == 0) return;

                try
                {
                    switch (op)
                    {
                        case 1:
                            var id = ReadNonEmpty("ID Estudiante: ");
                            Console.WriteLine(gestor.GenerarReporteEstudiante(id));
                            break;

                        case 2:
                            var top = gestor.ObtenerTop10Estudiantes();
                            SetInfo("Top 10:");
                            foreach (var x in top)
                                Console.WriteLine($"{x.Estudiante.Nombre} {x.Estudiante.Apellido} - {x.Promedio:0.00}");
                            break;

                        case 3:
                            var riesgo = gestor.ObtenerEstudiantesEnRiesgo();
                            SetInfo("En riesgo (<7):");
                            foreach (var x in riesgo)
                                Console.WriteLine($"{x.Estudiante.Nombre} {x.Estudiante.Apellido} - {x.Promedio:0.00}");
                            break;

                        case 4:
                            var pop = gestor.ObtenerCursosMasPopulares();
                            SetInfo("Cursos más populares:");
                            foreach (var c in pop)
                                Console.WriteLine($"{c.Curso.Codigo} - {c.Curso.Nombre}: {c.CantidadEstudiantes} estudiantes");
                            break;

                        case 5:
                            var prom = gestor.ObtenerPromedioGeneral();
                            Console.WriteLine($"Promedio General: {prom:0.00}");
                            break;

                        case 6:
                            var stats = gestor.ObtenerEstadisticasPorCarrera();
                            SetInfo("Estadísticas por carrera:");
                            foreach (var s in stats)
                                Console.WriteLine($"{s.Carrera}: {s.Cantidad} est. | Prom: {s.PromedioGeneral:0.00}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    SetError($"Error: {ex.Message}");
                }

                Console.WriteLine("Presiona una tecla para continuar...");
                Console.ReadKey();
            }
        }

        // ==== Análisis con Reflection ====
        static void AccionReflection()
        {
            Console.Clear();
            SetInfo("Análisis con Reflection");

            Console.WriteLine(AnalizadorReflection.MostrarPropiedades(typeof(Estudiante)));
            Console.WriteLine(AnalizadorReflection.MostrarMetodos(typeof(Estudiante)));
            Console.WriteLine();

            Console.WriteLine(AnalizadorReflection.MostrarPropiedades(typeof(Profesor)));
            Console.WriteLine(AnalizadorReflection.MostrarMetodos(typeof(Profesor)));
            Console.WriteLine();

            Console.WriteLine(AnalizadorReflection.MostrarPropiedades(typeof(Curso)));
            Console.WriteLine(AnalizadorReflection.MostrarMetodos(typeof(Curso)));
            Console.WriteLine();

            var e = AnalizadorReflection.CrearInstanciaDinamica(
                typeof(Estudiante),
                "EST-999", "Lia", "Torres", new DateTime(2004, 4, 12), "Informática", "2025-0999"
            );
            Console.WriteLine("Instancia creada dinámicamente:");
            Console.WriteLine(e);

            var rol = AnalizadorReflection.InvocarMetodo(e, "ObtenerRol");
            Console.WriteLine($"Rol por reflection: {rol}");

            Console.WriteLine("Presiona una tecla para continuar...");
            Console.ReadKey();
        }

    }
}

 

