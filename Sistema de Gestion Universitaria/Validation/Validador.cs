using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Sistema_de_Gestion_Universitaria.Validation.AtributoValidacion;

namespace Sistema_de_Gestion_Universitaria.Validation
{
    public static class Validador
    {
        public static List<string> Validar(object instancia)
        {
            if (instancia == null) throw new ArgumentNullException(nameof(instancia));

            var errores = new List<string>();
            var tipo = instancia.GetType();
            var props = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
            {
                var valor = p.GetValue(instancia);
                var nombreProp = $"{tipo.Name}.{p.Name}";

                // [Requerido]
                var req = p.GetCustomAttribute<AtributoValidacion>();
                if (req != null)
                {
                    if (EsNuloOVacio(valor))
                    {
                        errores.Add(req.Mensaje ?? $"{nombreProp} es requerido.");
                        continue; // si falta, no seguimos validando esta propiedad
                    }
                }

                // [ValidacionRango]
                var rango = p.GetCustomAttribute<ValidacionRangoAttribute>();
                if (rango != null && valor != null)
                {
                    if (!EsNumero(valor))
                    {
                        errores.Add($"{nombreProp} debe ser numérico para validar rango.");
                    }
                    else
                    {
                        var dec = Convert.ToDecimal(valor);
                        if (dec < rango.Minimo || dec > rango.Maximo)
                            errores.Add($"{nombreProp} debe estar entre {rango.Minimo} y {rango.Maximo}.");
                    }
                }

                // [Formato]
                var formato = p.GetCustomAttribute<FormatoAttribute>();
                if (formato != null && valor != null)
                {
                    var s = valor as string;
                    if (s == null)
                    {
                        errores.Add($"{nombreProp} debe ser cadena para validar formato.");
                    }
                    else
                    {
                        if (PareceRegex(formato.Patron))
                        {
                            if (!Regex.IsMatch(s, formato.Patron))
                                errores.Add(formato.Mensaje ?? $"{nombreProp} no cumple el formato requerido.");
                        }
                        else
                        {
                            var regex = TraducirPatronSimbolicoARegex(formato.Patron);
                            if (!Regex.IsMatch(s, regex))
                                errores.Add(formato.Mensaje ?? $"{nombreProp} no cumple el formato '{formato.Patron}'.");
                        }
                    }
                }
            }

            return errores;
        }

        private static bool EsNuloOVacio(object valor)
        {
            if (valor == null) return true;
            if (valor is string s) return string.IsNullOrWhiteSpace(s);
            if (valor is ICollection c) return c.Count == 0;
            return false;
        }

        private static bool EsNumero(object valor)
        {
            return valor is sbyte || valor is byte ||
                   valor is short || valor is ushort ||
                   valor is int || valor is uint ||
                   valor is long || valor is ulong ||
                   valor is float || valor is double ||
                   valor is decimal;
        }

        private static bool PareceRegex(string patron)
        {
            if (string.IsNullOrWhiteSpace(patron)) return false;
            return patron.StartsWith("^") || patron.Contains("\\d") || patron.Contains("[") || patron.Contains("(");
        }

        private static string TraducirPatronSimbolicoARegex(string patron)
        {
            // X → [A-Z], N → [0-9]. Ej: "XXX-XXXXX" -> "^[A-Z]{3}-[A-Z]{5}$"
            if (string.IsNullOrEmpty(patron)) return "^.*$";
            int countX = 0, countN = 0;
            var rx = "^";
            foreach (var ch in patron)
            {
                if (ch == 'X') { countX++; continue; }
                if (countX > 0) { rx += $"[A-Z]{{{countX}}}"; countX = 0; }

                if (ch == 'N') { countN++; continue; }
                if (countN > 0) { rx += $"[0-9]{{{countN}}}"; countN = 0; }

                rx += Regex.Escape(ch.ToString());
            }
            if (countX > 0) rx += $"[A-Z]{{{countX}}}";
            if (countN > 0) rx += $"[0-9]{{{countN}}}";
            rx += "$";
            return rx;
        }
    }

}
