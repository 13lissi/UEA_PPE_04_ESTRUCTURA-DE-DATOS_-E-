namespace SistemaVuelos
{
    // Representa un vuelo (arista dirigida del grafo)
    public class Vuelo
    {
        public string Origen { get; }
        public string Destino { get; }
        public double Precio { get; }
        public string Aerolinea { get; }
        public double Duracion { get; }

        public Vuelo(string origen, string destino, double precio, string aerolinea, double duracion)
        {
            Origen = origen;
            Destino = destino;
            Precio = precio;
            Aerolinea = aerolinea;
            Duracion = duracion;
        }

        public override string ToString()
        {
            return $"  → {Destino,-20} | ${Precio,6:F0} | {Aerolinea,-20} | {Duracion}h";
        }
    }
}