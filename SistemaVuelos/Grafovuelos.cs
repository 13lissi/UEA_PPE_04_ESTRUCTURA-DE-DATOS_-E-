using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaVuelos
{
    public class GrafoVuelos
    {
        // OrdinalIgnoreCase aqui: "Loja" == "loja" == "LOJA" siempre *(para recordar)*
        private readonly Dictionary<string, List<Vuelo>> _adyacencia =
            new(StringComparer.OrdinalIgnoreCase);

        public void AgregarVuelo(Vuelo vuelo)
        {
            if (!_adyacencia.ContainsKey(vuelo.Origen))
                _adyacencia[vuelo.Origen] = [];
            _adyacencia[vuelo.Origen].Add(vuelo);
        }

        public void CargarVuelos(IEnumerable<Vuelo> vuelos)
        {
            foreach (var v in vuelos)
                AgregarVuelo(v);
        }

        // Devuelve vuelos saliendo de esa ciudad (case-insensitive)
        public List<Vuelo> VuelosDesde(string ciudad)
        {
            return _adyacencia.TryGetValue(ciudad, out var lista)
                ? lista
                : new List<Vuelo>();
        }

        // Todas las ciudades que aparecen como origen O destino
        public HashSet<string> ObtenerCiudades()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in _adyacencia)
            {
                set.Add(kv.Key);
                foreach (var v in kv.Value)
                    set.Add(v.Destino);
            }
            return set;
        }

        public Dictionary<string, List<Vuelo>> ObtenerTodo() => _adyacencia;
        public int TotalVuelos() => _adyacencia.Values.Sum(l => l.Count);

        // Filtros de reportería — siempre ordenados de más barato a más caro
        public List<Vuelo> VuelosPorAerolinea(string aerolinea) =>
            _adyacencia.Values
                .SelectMany(l => l)
                .Where(v => v.Aerolinea.Contains(aerolinea, StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v.Precio)
                .ToList();

        public List<Vuelo> VuelosPorPrecioMaximo(double max) =>
            _adyacencia.Values
                .SelectMany(l => l)
                .Where(v => v.Precio <= max)
                .OrderBy(v => v.Precio)
                .ToList();

        public List<Vuelo> VuelosMasBaratos(int top) =>
            [.. _adyacencia.Values
                .SelectMany(l => l)
                .OrderBy(v => v.Precio)
                .Take(top)];

        // Dijkstra — ruta de menor costo
        public ResultadoRuta Dijkstra(string origen, string destino)
        {
            if (origen.Equals(destino, StringComparison.OrdinalIgnoreCase))
                return new ResultadoRuta(0, new List<string> { origen });

            var cola   = new PriorityQueue<(string Ciudad, List<string> Ruta), double>();
            var costos = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            cola.Enqueue((origen, new List<string> { origen }), 0);
            costos[origen] = 0;

            while (cola.Count > 0)
            {
                var (actual, ruta) = cola.Dequeue();

                if (actual.Equals(destino, StringComparison.OrdinalIgnoreCase))
                    return new ResultadoRuta(costos[destino], ruta);

                foreach (var vuelo in VuelosDesde(actual))
                {
                    double nuevo = costos[actual] + vuelo.Precio;
                    if (!costos.TryGetValue(vuelo.Destino, out double existente) || nuevo < existente)
                    {
                        costos[vuelo.Destino] = nuevo;
                        cola.Enqueue((vuelo.Destino, new List<string>(ruta) { vuelo.Destino }), nuevo);
                    }
                }
            }

            return new ResultadoRuta(double.MaxValue, new List<string>());
        }
    }
}