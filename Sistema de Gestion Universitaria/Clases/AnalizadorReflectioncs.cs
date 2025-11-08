using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_Universitaria.Clases
{
    public static class AnalizadorReflection
    {
        public static string MostrarPrpiedades(Type tipo)

        {
            if (tipo == null) throw new ArgumentNullException(nameof(tipo));

            var props = tipo.GetProperties(BindingFlags.Public | System.Reflection.BindingFlags.Instance | BindingFlags.Static);
            if (props.Length == 0)
                return $"Tipo {tipo.Name} no tiene propiedades publicas.";

            var lineas = props
                .Select(p => $"- {p.PropertyType.Name} {p.Name}")
                .ToList();

            return $"Propiedades de {tipo.Name}:\n " + string.Join("\n", lineas);
        }

        public static string MostrarMetodos(Type tipo)
        {
            if (tipo == null)
                throw new ArgumentNullException(nameof(tipo));

            var metodos = tipo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(m => m.DeclaringType != typeof(object))
                .ToArray();

            if (metodos.Length == 0)
                return $"Tipo {tipo.Name} no tiene metodos publicos (aparte de Object).";

            string Firma(MethodInfo m)
            {
                var parametros = m.GetParameters();
                var pars = parametros.Length == 0
                    ? ""
                    : string.Join(", ", parametros.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                return $"{m.ReturnType.Name} {m.Name}({pars})";
            }

            var lineas = metodos.Select(Firma).ToList();
            return $"Metodos de {tipo.Name}:\n " + string.Join("\n", lineas);

        }

        public static object CrearInstancia(Type tipo, params object[] parametros)
        {
            if (tipo == null)
                throw new ArgumentNullException(nameof(tipo));
            return Activator.CreateInstance(tipo, parametros);


        }

        public static object InvocarMetodo(object instancia, string nombreMetodo, params object[] parametros)
        {
            if (instancia == null)
                throw new ArgumentNullException(nameof(instancia));
            if (string.IsNullOrWhiteSpace(nombreMetodo))
                throw new ArgumentException("Nombre de metodo vacio", nameof(nombreMetodo));

            var tipo = instancia.GetType();
            var metodo = tipo.GetMethod(nombreMetodo, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (metodo == null)
                throw new MissingMethodException($"No se encontró el método '{nombreMetodo}' en {tipo.Name}");
            return metodo.Invoke(instancia, parametros);
        }

    }
}
