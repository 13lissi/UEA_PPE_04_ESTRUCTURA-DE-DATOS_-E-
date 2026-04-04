namespace SistemaVuelos
{
    // Conecta la interfaz de usuario con la lógica del grafo
    // Principio de Responsabilidad Única: solo coordina, no procesa ni muestra
    public class ControladorVuelos
    {
        private readonly GrafoVuelos _grafo;

        public ControladorVuelos()
        {
            _grafo = new GrafoVuelos();
            _grafo.CargarVuelos(CargadorVuelos.Cargar());
        }

        // ---- Consultas generales ----
        public HashSet<string> ObtenerCiudades() => _grafo.ObtenerCiudades();
        public int TotalVuelos() => _grafo.TotalVuelos();
        public int TotalCiudades() => _grafo.ObtenerCiudades().Count;

        // ---- Reportería ----
        public List<Vuelo> VuelosDesde(string ciudad)     => _grafo.VuelosDesde(ciudad);
        public Dictionary<string, List<Vuelo>> RedCompleta() => _grafo.ObtenerTodo();

        public List<Vuelo> FiltrarPorAerolinea(string aerolinea)
            => _grafo.VuelosPorAerolinea(aerolinea);

        public List<Vuelo> FiltrarPorPrecioMaximo(double precio)
            => _grafo.VuelosPorPrecioMaximo(precio);

        public List<Vuelo> VuelosMasBaratos(int top = 10)
            => _grafo.VuelosMasBaratos(top);

        // ---- Búsqueda ----
        public ResultadoRuta BuscarRuta(string origen, string destino)
            => _grafo.Dijkstra(origen, destino);
    }
}