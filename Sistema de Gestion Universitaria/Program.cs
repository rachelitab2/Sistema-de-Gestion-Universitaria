using System;
using Sistema_de_Gestion_Universitaria.Clases;
using Sistema_de_Gestion_Universitaria.Validation;

namespace Sistema_de_Gestion_Universitaria
{
    internal class Program
    {
        static Repositorio<Estudiante> repoEstudiante = new Repositorio<Estudiante>();
        static Repositorio<Profesor> repoProfesor = new Repositorio<Profesor>();
        static Repositorio<Curso> repoCurso = new Repositorio<Curso>();
        static GestorMatriculas gestor = new GestorMatriculas(repoEstudiante, repoCurso);


        static void Main(string[] args)
        {
            Console.Title = "Sistema de Gestion Universitaria";

            GenerarDatosPrueba();
            DemostrarFuncionalidades();

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

        // ==== Menú Principal ====
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
                                Console.WriteLine($"{c.Curso.Codigo} - {c.Curso.Nombre}: {c.Cantidad} estudiantes");
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

        static void GenerarDatosPrueba()
        {
            var rnd = new Random(123);

            var profs = new List<Profesor>
    {
        new Profesor { Identificacion = "PROF-001", Nombre = "Marta", Apellido = "Suárez", FechaNacimiento = new DateTime(1980,3,5), Departamento = "Computación", TipoContrato = TipoContrato.TiempoCompleto, SalarioBase = 45000m },
        new Profesor { Identificacion = "PROF-002", Nombre = "Luis", Apellido = "Pérez", FechaNacimiento = new DateTime(1978,7,21), Departamento = "Matemáticas", TipoContrato = TipoContrato.MedioTiempo, SalarioBase = 30000m },
        new Profesor { Identificacion = "PROF-003", Nombre = "Sara", Apellido = "Gil", FechaNacimiento = new DateTime(1982,10,10), Departamento = "Física", TipoContrato = TipoContrato.TiempoCompleto, SalarioBase = 42000m },
        new Profesor { Identificacion = "PROF-004", Nombre = "Carlos", Apellido = "Lora", FechaNacimiento = new DateTime(1975,1,15), Departamento = "Química", TipoContrato = TipoContrato.Adjunto, SalarioBase = 20000m },
        new Profesor { Identificacion = "PROF-005", Nombre = "Elena", Apellido = "Vargas", FechaNacimiento = new DateTime(1983,12,2), Departamento = "Derecho", TipoContrato = TipoContrato.TiempoCompleto, SalarioBase = 50000m },
    };
            foreach (var p in profs) repoProfesores.Agregar(p);

            var cursos = new List<Curso>
    {
        new Curso("INF-101","Programación I",4, profs[0]),
        new Curso("INF-102","Programación II",4, profs[0]),
        new Curso("MAT-101","Cálculo I",4, profs[1]),
        new Curso("MAT-102","Álgebra",3, profs[1]),
        new Curso("FIS-101","Física I",4, profs[2]),
        new Curso("QUI-101","Química I",4, profs[3]),
        new Curso("DER-101","Introducción al Derecho",3, profs[4]),
        new Curso("INF-201","Estructuras de Datos",4, profs[0]),
        new Curso("INF-202","Bases de Datos",4, profs[0]),
        new Curso("INF-203","Ingeniería de Software",3, profs[0]),
    };
            foreach (var c in cursos) repoCursos.Agregar(c);

            var carreras = new[] { "Informática", "Derecho", "Matemáticas", "Física", "Química" };
            var nombres = new[] { "Ana", "Luis", "Carlos", "María", "Elena", "Jorge", "Lucía", "Pedro", "Daniela", "Sofía", "Raúl", "Carmen", "Iván", "Noelia", "Tomás" };
            var apellidos = new[] { "Gómez", "Pérez", "Linares", "Suárez", "Mejía", "Vargas", "Lora", "Núñez", "Gil", "Torres", "Mena", "Silva", "Rojas", "Acosta", "Santos" };

            for (int i = 0; i < 15; i++)
            {
                var nombre = nombres[i % nombres.Length];
                var apellido = apellidos[i % apellidos.Length];
                var carrera = carreras[i % carreras.Length];
                var year = 1999 + rnd.Next(0, 6);
                var mes = rnd.Next(1, 13);
                var dia = rnd.Next(1, 28);
                var est = new Estudiante
                {
                    Identificacion = $"EST-{(i + 1).ToString("000")}",
                    Nombre = nombre,
                    Apellido = apellido,
                    FechaNacimiento = new DateTime(year, mes, dia),
                    Carrera = carrera,
                    NumeroMatricula = $"2025-{(1000 + i).ToString("0000")}"
                };
                repoEstudiantes.Agregar(est);
            }

            int matriculasCreadas = 0;
            int intentos = 0;
            var estudiantes = repoEstudiantes.ObtenerTodos();

            while (matriculasCreadas < 30 && intentos < 200)
            {
                intentos++;
                var est = estudiantes[rnd.Next(estudiantes.Count)];
                var cur = cursos[rnd.Next(cursos.Count)];

                try
                {
                    gestor.MatricularEstudiante(est, cur);

                    int notas = 3 + rnd.Next(2);
                    for (int k = 0; k < notas; k++)
                    {
                        var nota = Math.Round((decimal)(6.0 + rnd.NextDouble() * 4.0), 2);
                        if (nota > 10m) nota = 10m;
                        gestor.AgregarCalificacion(est.Identificacion, cur.Codigo, nota);
                    }

                    matriculasCreadas++;
                }
                catch
                {
                }
            }
        }

        static void DemostrarFuncionalidades()
        {
            SetInfo("Demostración automática de funcionalidades");

            var top10 = gestor.ObtenerTop10Estudiantes();
            Console.WriteLine("Top 10 Estudiantes:");
            foreach (var x in top10)
                Console.WriteLine($"{x.Estudiante.Identificacion} - {x.Estudiante.Nombre} {x.Estudiante.Apellido} - {x.Promedio:0.00}");

            var riesgo = gestor.ObtenerEstudiantesEnRiesgo();
            Console.WriteLine("Estudiantes en Riesgo (<7.0):");
            foreach (var x in riesgo)
                Console.WriteLine($"{x.Estudiante.Identificacion} - {x.Estudiante.Nombre} {x.Estudiante.Apellido} - {x.Promedio:0.00}");

            var populares = gestor.ObtenerCursosMasPopulares();
            Console.WriteLine("Cursos más Populares:");
            foreach (var c in populares)
                Console.WriteLine($"{c.Curso.Codigo} - {c.Curso.Nombre}: {c.Cantidad}");

            var promGeneral = gestor.ObtenerPromedioGeneral();
            Console.WriteLine($"Promedio General del Sistema: {promGeneral:0.00}");

            var statsCarrera = gestor.ObtenerEstadisticasPorCarrera();
            Console.WriteLine("Estadísticas por Carrera:");
            foreach (var s in statsCarrera)
                Console.WriteLine($"{s.Carrera}: {s.Cantidad} est. | Prom: {s.PromedioGeneral:0.00}");

            var alguno = repoEstudiantes.ObtenerTodos().FirstOrDefault();
            if (alguno != null)
            {
                Console.WriteLine();
                Console.WriteLine("Reporte de un estudiante al azar:");
                Console.WriteLine(gestor.GenerarReporteEstudiante(alguno.Identificacion));
            }

            Console.WriteLine();
            Console.WriteLine("Reflection sobre Estudiante:");
            Console.WriteLine(AnalizadorReflection.MostrarPropiedades(typeof(Estudiante)));
            Console.WriteLine(AnalizadorReflection.MostrarMetodos(typeof(Estudiante)));

            var e = AnalizadorReflection.CrearInstanciaDinamica(
                typeof(Estudiante),
                "EST-999", "Lia", "Torres", new DateTime(2004, 4, 12), "Informática", "2025-0999"
            );
            Console.WriteLine("Instancia Estudiante creada dinámicamente:");
            Console.WriteLine(e);
            var rol = AnalizadorReflection.InvocarMetodo(e, "ObtenerRol");
            Console.WriteLine($"Rol por reflection: {rol}");

            Console.WriteLine();
            Console.WriteLine("Validación con atributos (caso con errores):");
            var estBad = new Estudiante
            {
                Identificacion = "EST-300",
                Nombre = "Joan",
                Apellido = "Mejía",
                FechaNacimiento = new DateTime(2006, 5, 10),
                Carrera = "",
                NumeroMatricula = "25-001"
            };
            var erroresEst = Validador.Validar(estBad);
            foreach (var err in erroresEst) Console.WriteLine("- " + err);

            Console.WriteLine();
            Console.WriteLine("Boxing/Unboxing y Conversiones:");
            decimal nota = 8.75m;
            object caja = nota;
            decimal nota2 = (decimal)caja;
            Console.WriteLine($"Boxing/Unboxing - Original: {nota}, Recuperado: {nota2}");

            Console.WriteLine(Utilidades.ConvertirDatos(123));
            Console.WriteLine(Utilidades.ConvertirDatos(3.14159));
            Console.WriteLine(Utilidades.ConvertirDatos(42.5m));
            Console.WriteLine(Utilidades.ConvertirDatos("Hola"));
            Console.WriteLine(Utilidades.ConvertirDatos(""));
            Console.WriteLine(Utilidades.ConvertirDatos(DateTime.Today));
            Console.WriteLine(Utilidades.ConvertirDatos(true));

            Console.WriteLine();
            Console.WriteLine("Fin de demostración. Presiona una tecla para ir al menú...");
            Console.ReadKey();
        }
    }
}

 

