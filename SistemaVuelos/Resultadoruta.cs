using System.Collections.Generic;

namespace SistemaVuelos
{
    // Encapsula el resultado del algoritmo de Dijkstra
    public class ResultadoRuta
    {
        public double Costo { get; }
        public List<string> Ruta { get; }
        public bool Encontrada => Costo < double.MaxValue && Ruta.Count > 0;

        public ResultadoRuta(double costo, List<string> ruta)
        {
            Costo = costo;
            Ruta = ruta;
        }
    }
}