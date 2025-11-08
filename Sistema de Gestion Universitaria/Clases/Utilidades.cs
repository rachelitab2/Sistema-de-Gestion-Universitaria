using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_Universitaria.Clases
{
    public static class Utilidades
    {
        public static string ConvertirDatos(object dato)
        {
            switch (dato)
            {
                case null:
                    return "Valor: null";
                case int i:
                    return $"Entero: {i}";
                case double d:
                    return $"Double: {d.ToString("0.###", CultureInfo.InvariantCulture)}";
                case decimal m:
                    return $"Decimal: {m:0.##}";
                case string s when string.IsNullOrWhiteSpace(s):
                    return "Cadena: (vacia)";
                case string s:
                    return $"Cadena: \"{s}\"";
                case DateTime dt:
                    return $"Fecha: {dt:yyyy-MM-dd}";
                case bool b:
                    return $"Booleano: {(b ? "true" : "false")}";
                default:
                    return $"Tipo {dato.GetType().Name}: {dato}";
            }
        }

        public static bool ParsearCalificacion(string entrada, out decimal calificacion)
        {
            calificacion = 0m;

            if (string.IsNullOrWhiteSpace(entrada))
                return false;
            var normalizada = entrada.Trim().Replace(',', '.');

            if (!decimal.TryParse(normalizada, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor))
                return false;


            if (valor < 0m || valor > 10m)
                return false;

            calificacion = valor;
            return true;


        }
    }
}
