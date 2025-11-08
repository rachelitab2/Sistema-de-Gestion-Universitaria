using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Gestion_Universitaria.Interfaces;

namespace Sistema_Gestion_Universitaria.Clases
{
    public class Repositorio<T> where T : IIdentificable
    {
        public readonly Dictionary<string, T> _items;

        public Repositorio()
        {
            _items = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }

        public void Agregar(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrWhiteSpace(item.Identificacion))
                throw new ArgumentException("El Elemento debe tener una identificacion valida");

            if (_items.ContainsKey(item.Identificacion))
                throw new InvalidOperationException("Ya existe un elemento con esa identificacion");

            _items.Add(item.Identificacion, item);

        }

        public void Eliminar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("El id no puede estar vacio");

            if (!_items.Remove(id))
                throw new KeyNotFoundException("No existe un elemento con ese id");

        }

        public T BuscarPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return default;
            return _items.TryGetValue(id, out var value) ? value : default;

        }

        public List<T> Buscar(Func<T, bool> predicado)
        {
            if (predicado == null) throw new ArgumentNullException(nameof(predicado));
            return _items.Values.Where(predicado).ToList();
        }

        public List<T> ObtenerTodos() => _items.Values.ToList();
    }


}
