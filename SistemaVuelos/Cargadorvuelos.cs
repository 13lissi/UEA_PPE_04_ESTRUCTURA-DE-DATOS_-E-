namespace SistemaVuelos
{
    // Lee el archivo vuelos.txt y devuelve una lista de Vuelo
    // Si el archivo no existe, usa datos de respaldo internos
    public static class CargadorVuelos
    {
        private const string RutaArchivo = "vuelos.txt";

        public static List<Vuelo> Cargar()
        {
            if (File.Exists(RutaArchivo))
            {
                try
                {
                    return LeerDesdeArchivo(RutaArchivo);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Advertencia] No se pudo leer {RutaArchivo}: {ex.Message}");
                    Console.WriteLine("  Usando base de datos interna de respaldo...");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Info] Archivo {RutaArchivo} no encontrado. Usando datos internos.");
                Console.ResetColor();
            }

            return DatosRespaldo();
        }

        // -------------------------------------------------------
        // Parseo del archivo de texto
        // -------------------------------------------------------
        private static List<Vuelo> LeerDesdeArchivo(string ruta)
        {
            var vuelos = new List<Vuelo>();
            int linea = 0;

            foreach (var texto in File.ReadLines(ruta))
            {
                linea++;
                var trimmed = texto.Trim();

                // Ignorar líneas vacías y comentarios
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                    continue;

                var partes = trimmed.Split('|');
                if (partes.Length != 5)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  [Línea {linea}] Formato inválido: ignorada.");
                    Console.ResetColor();
                    continue;
                }

                if (!double.TryParse(partes[2].Trim(), out double precio) ||
                    !double.TryParse(partes[4].Trim(), out double duracion))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  [Línea {linea}] Precio o duración no numérico: ignorada.");
                    Console.ResetColor();
                    continue;
                }

                vuelos.Add(new Vuelo(
                    partes[0].Trim(),
                    partes[1].Trim(),
                    precio,
                    partes[3].Trim(),
                    duracion
                ));
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {vuelos.Count} vuelos cargados desde {ruta}");
            Console.ResetColor();
            return vuelos;
        }

        // -------------------------------------------------------
        // Datos mínimos de respaldo (Ecuador + vecinos)
        // -------------------------------------------------------
        private static List<Vuelo> DatosRespaldo()
        {
            return new List<Vuelo>
            {
                new("Quito",     "Guayaquil", 65,  "Latam",            1),
                new("Quito",     "Bogota",    150, "Latam",            2),
                new("Guayaquil", "Quito",     60,  "Latam",            1),
                new("Guayaquil", "Miami",     310, "American Airlines", 4),
                new("Bogota",    "Lima",      190, "Avianca",          3),
                new("Lima",      "Santiago",  180, "Sky",              3),
                new("Santiago",  "Buenos Aires", 120, "JetSmart",      2),
                new("Miami",     "New York",  150, "Delta",            3),
                new("New York",  "Madrid",    490, "Iberia",           8),
                new("Madrid",    "Paris",     120, "Air France",       2),
            };
        }
    }
}