using System.Diagnostics;

namespace SistemaVuelos
{
    public class ConsolaUI
    {
        private readonly ControladorVuelos _ctrl;
        // Lista canónica de ciudades tal como están en el txt, ordenadas
        private readonly List<string> _ciudades;

        public ConsolaUI(ControladorVuelos controlador)
        {
            _ctrl    = controlador;
            _ciudades = _ctrl.ObtenerCiudades().OrderBy(c => c).ToList();
        }

        // ================================================================
        // MENÚ PRINCIPAL
        // ================================================================
        public void Iniciar()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            MostrarBienvenida(); // ya espera tecla dentro

            while (true)
            {
                Console.Clear();
                MostrarMenuPrincipal();
                var op = Console.ReadLine()?.Trim();

                switch (op)
                {
                    case "1": VerCiudades();         break;
                    case "2": VerVuelosDesde();      break;
                    case "3": VerRedCompleta();      break;
                    case "4": FiltrarPorPrecio();    break;
                    case "5": VerTopVuelos();        break; 
                    case "6": BuscarRutaBarata();    break; 
                    case "7": FiltrarPorAerolinea(); break;
                    case "8": MostrarDespedida(); return;
                    default:
                        Alerta("Opción inválida. Elige un número del 1 al 8.");
                        Pausa();
                        break;
                }
            }
        }

        // ================================================================
        // 1 — Ver todas las ciudades
        // ================================================================
        private void VerCiudades()
        {
            Titulo("CIUDADES DISPONIBLES");
            Info($"  {_ciudades.Count} ciudades en la base de datos:\n");
            ImprimirCiudadesEnColumnas(_ciudades);
            Pausa();
        }

        // ================================================================
        // 2 — Vuelos desde ciudad (el usuario escribe)
        // ================================================================
        private void VerVuelosDesde()
        {
            Titulo("VUELOS DESDE UNA CIUDAD");

            var conSalidas = _ctrl.RedCompleta().Keys.OrderBy(c => c).ToList();
            Info("  Ciudades con vuelos de salida disponibles:\n");
            ImprimirCiudadesEnColumnas(conSalidas);

            var ciudad = PedirCiudad("Ciudad de origen", conSalidas);
            if (ciudad == null) { Pausa(); return; }

            Console.Clear();
            Titulo($"VUELOS DESDE {ciudad.ToUpper()}");

            var vuelos = _ctrl.VuelosDesde(ciudad).OrderBy(v => v.Precio).ToList();
            Info($"  {vuelos.Count} conexión(es) directa(s) — de menor a mayor precio:\n");
            EncabezadoVuelos();
            foreach (var v in vuelos) FilaVuelo(v);
            Pausa();
        }

        // ================================================================
        // 3 — Ruta más barata entre
        // ================================================================
        private void BuscarRutaBarata()
        {
            Titulo("BUSCAR RUTA MÁS ECONÓMICA");

            Info("  Ciudades disponibles:\n");
            ImprimirCiudadesEnColumnas(_ciudades);

            // Paso 1: origen
            var origen = PedirCiudad("Ciudad de ORIGEN", _ciudades);
            if (origen == null) { Pausa(); return; }

            // Paso 2: destino (sin el origen)
            Console.Clear();
            Titulo("BUSCAR RUTA MÁS ECONÓMICA");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Origen seleccionado: {origen}");
            Console.ResetColor();
            Console.WriteLine();

            var sinOrigen = _ciudades.Where(c =>
                !c.Equals(origen, StringComparison.OrdinalIgnoreCase)).ToList();

            Info("  Ciudades disponibles como destino:\n");
            ImprimirCiudadesEnColumnas(sinOrigen);

            var destino = PedirCiudad("Ciudad de DESTINO", sinOrigen);
            if (destino == null) { Pausa(); return; }

            // Ejecutar
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  Calculando: {origen} → {destino}...");
            Console.ResetColor();

            var sw = Stopwatch.StartNew();
            var r  = _ctrl.BuscarRuta(origen, destino);
            sw.Stop();

            Console.Clear();
            Titulo("RESULTADO");

            if (!r.Encontrada)
            {
                Alerta($"No existe ruta entre '{origen}' y '{destino}'.");
                Pausa();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║         RUTA MÁS ECONÓMICA               ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("\n  Ruta:        ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Join(" → ", r.Ruta));
            Console.ResetColor();

            int escalas = r.Ruta.Count - 2;
            Console.Write("  Escalas:     ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(escalas <= 0 ? "Vuelo directo" : $"{escalas} escala(s)");
            Console.ResetColor();

            Console.Write("  Costo total: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"${r.Costo:F0}");
            Console.ResetColor();

            Console.Write("  Tiempo:      ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
            Console.ResetColor();

            Pausa();
        }

        // ================================================================
        // 4 — Red completa
        // ================================================================
        private void VerRedCompleta()
        {
            Titulo("RED COMPLETA DE VUELOS");
            Info($"  Total de rutas: {_ctrl.TotalVuelos()}\n");

            foreach (var kv in _ctrl.RedCompleta().OrderBy(k => k.Key))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n  ▶ {kv.Key}  ({kv.Value.Count} vuelo(s)):");
                Console.ResetColor();
                EncabezadoVuelos();
                foreach (var v in kv.Value.OrderBy(x => x.Precio))
                    FilaVuelo(v);
            }
            Pausa();
        }

        // ================================================================
        // 5 — Buscar por aerolínea (usuario escribe, búsqueda parcial)
        // ================================================================
        private void FiltrarPorAerolinea()
        {
            Titulo("BUSCAR POR AEROLÍNEA");

            // Lista de aerolíneas reales del grafo
            var aerolineas = _ctrl.RedCompleta().Values
                .SelectMany(l => l)
                .Select(v => v.Aerolinea)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(a => a)
                .ToList();

            Info($"  Aerolíneas disponibles ({aerolineas.Count}):\n");
            foreach (var a in aerolineas)
                Console.WriteLine($"    • {a}");
            Console.WriteLine();

            string? elegida = null;
            int intentos = 0;

            while (elegida == null && intentos < 3)
            {
                Console.Write("  Escribe nombre o parte del nombre (0 = cancelar): ");
                var input = Console.ReadLine()?.Trim() ?? "";

                if (input == "0") { Pausa(); return; }
                if (string.IsNullOrWhiteSpace(input))
                {
                    Alerta("Entrada vacía. Inténtalo de nuevo.");
                    intentos++;
                    continue;
                }

                var coincidencias = aerolineas
                    .Where(a => a.Contains(input, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (coincidencias.Count == 0)
                {
                    intentos++;
                    int restantes = 3 - intentos;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  ✗ No se encontró '{input}' en la lista.");
                    Console.ResetColor();
                    if (restantes > 0)
                        Console.WriteLine($"    Te quedan {restantes} intento(s). Revisa la lista de arriba.");
                }
                else if (coincidencias.Count == 1)
                {
                    elegida = coincidencias[0];
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  → Aerolínea encontrada: {elegida}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  Varias coincidencias para '{input}':");
                    Console.ResetColor();
                    for (int i = 0; i < coincidencias.Count; i++)
                        Console.WriteLine($"    [{i + 1}] {coincidencias[i]}");
                    Console.Write("\n  Número (0 = cancelar): ");
                    var sel = Console.ReadLine()?.Trim() ?? "0";
                    if (int.TryParse(sel, out int idx) && idx >= 1 && idx <= coincidencias.Count)
                        elegida = coincidencias[idx - 1];
                    else if (sel != "0")
                    {
                        Alerta("Número inválido.");
                        intentos++;
                    }
                    else { Pausa(); return; }
                }
            }

            if (elegida == null) { Alerta("Demasiados intentos. Operación cancelada."); Pausa(); return; }

            var vuelos = _ctrl.FiltrarPorAerolinea(elegida); // ordenados por precio ASC

            Console.Clear();
            Titulo($"VUELOS DE {elegida.ToUpper()}");
            Info($"  {vuelos.Count} ruta(s) — de menor a mayor precio:\n");
            EncabezadoVuelos(conOrigen: true);
            foreach (var v in vuelos) FilaVueloConOrigen(v);
            Pausa();
        }

        // ================================================================
        // 6 — Filtrar por precio máximo
        // ================================================================
        private void FiltrarPorPrecio()
        {
            Titulo("FILTRAR POR PRECIO MÁXIMO");

            int[] rangos = { 100, 200, 300, 500, 1000 };
            Info("  Rangos sugeridos:\n");
            for (int i = 0; i < rangos.Length; i++)
            {
                int n = _ctrl.FiltrarPorPrecioMaximo(rangos[i]).Count;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"    [{i + 1}] ");
                Console.ResetColor();
                Console.WriteLine($"Hasta ${rangos[i],-5}  ({n} vuelo(s))");
            }
            Console.WriteLine();
            Console.Write("  Elige rango [1-5] o escribe precio en $ (0 = cancelar): ");
            var input = Console.ReadLine()?.Trim() ?? "0";

            double precioMax;
            if (int.TryParse(input, out int op) && op >= 1 && op <= rangos.Length)
                precioMax = rangos[op - 1];
            else if (double.TryParse(input, out double libre) && libre > 0)
                precioMax = libre;
            else { if (input != "0") Alerta("Valor inválido."); Pausa(); return; }

            var vuelos = _ctrl.FiltrarPorPrecioMaximo(precioMax); // ordenados por precio ASC

            Console.Clear();
            Titulo($"VUELOS HASTA ${precioMax:F0}");

            if (vuelos.Count == 0) { Alerta($"No hay vuelos ≤ ${precioMax:F0}."); Pausa(); return; }

            Info($"  {vuelos.Count} vuelo(s) — de menor a mayor precio:\n");
            EncabezadoVuelos(conOrigen: true);
            foreach (var v in vuelos) FilaVueloConOrigen(v);
            Pausa();
        }

        // ================================================================
        // 7 — Top 10 más baratos
        // ================================================================
        private void VerTopVuelos()
        {
            Titulo("TOP 10 VUELOS MÁS BARATOS");
            var vuelos = _ctrl.VuelosMasBaratos(10);

            Info("  Los 10 vuelos más económicos de toda la red:\n");
            EncabezadoVuelos(conOrigen: true);

            int rank = 1;
            foreach (var v in vuelos)
            {
                Console.ForegroundColor = rank == 1 ? ConsoleColor.Yellow
                                        : rank <= 3 ? ConsoleColor.Green
                                        : ConsoleColor.White;
                Console.WriteLine($"  #{rank,-2}  {v.Origen,-18} → {v.Destino,-18} ${v.Precio,6:F0}   {v.Aerolinea,-20} {v.Duracion}h");
                Console.ResetColor();
                rank++;
            }
            Pausa();
        }

        // ================================================================
        // BÚSQUEDA DE CIUDAD CON MANEJO DE ERRORES
        // Compara directamente contra los nombres reales del txt
        // ================================================================
        private static string? PedirCiudad(string etiqueta, List<string> validas)
        {
            int intentos = 0;
            const int max = 3;

            while (intentos < max)
            {
                Console.Write($"\n  {etiqueta} (0 = cancelar): ");
                var raw = Console.ReadLine()?.Trim() ?? "";

                if (raw == "0") return null;

                if (string.IsNullOrWhiteSpace(raw))
                {
                    Alerta("No escribiste nada. Inténtalo de nuevo.");
                    intentos++;
                    continue;
                }

                // Resolver alias primero
                var input = Normalizador.Limpiar(raw);

                // 1. Coincidencia exacta (case-insensitive) contra nombres del txt
                var exacta = validas.FirstOrDefault(c =>
                    c.Equals(input, StringComparison.OrdinalIgnoreCase));
                if (exacta != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  → {exacta}");
                    Console.ResetColor();
                    return exacta;
                }

                // 2. Coincidencia parcial (el input está contenido en el nombre)
                var parciales = validas.Where(c =>
                    c.Contains(input, StringComparison.OrdinalIgnoreCase)).ToList();

                if (parciales.Count == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  → Entendido como: {parciales[0]}");
                    Console.ResetColor();
                    return parciales[0];
                }

                if (parciales.Count > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  Varias ciudades coinciden con '{raw}':");
                    Console.ResetColor();
                    for (int i = 0; i < parciales.Count; i++)
                        Console.WriteLine($"    [{i + 1}] {parciales[i]}");
                    Console.Write("\n  Número (0 = cancelar): ");
                    var sel = Console.ReadLine()?.Trim() ?? "0";
                    if (int.TryParse(sel, out int idx) && idx >= 1 && idx <= parciales.Count)
                        return parciales[idx - 1];
                    if (sel != "0") { Alerta("Número inválido."); intentos++; }
                    else return null;
                    continue;
                }

                // 3. Sin coincidencias
                intentos++;
                int restantes = max - intentos;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  ✗ Ciudad '{raw}' no encontrada en la base de datos.");
                Console.ResetColor();
                if (restantes > 0)
                    Console.WriteLine($"    Revisa la lista de arriba. Te quedan {restantes} intento(s).");
            }

            Alerta("Demasiados intentos. Operación cancelada.");
            return null;
        }

        // ================================================================
        // HELPERS DE PRESENTACIÓN
        // ================================================================
        private static void ImprimirCiudadesEnColumnas(List<string> lista)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                Console.Write($"  {lista[i],-22}");
                if ((i + 1) % 3 == 0) Console.WriteLine();
            }
            if (lista.Count % 3 != 0) Console.WriteLine();
            Console.WriteLine();
        }

        private static void FilaVuelo(Vuelo v)
        {
            Console.WriteLine($"  {v.Destino,-22} ${v.Precio,6:F0}   {v.Aerolinea,-22} {v.Duracion}h");
        }

        private static void FilaVueloConOrigen(Vuelo v)
        {
            Console.WriteLine($"  {v.Origen,-18} → {v.Destino,-18} ${v.Precio,6:F0}   {v.Aerolinea,-20} {v.Duracion}h");
        }

        private static void EncabezadoVuelos(bool conOrigen = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (conOrigen)
                Console.WriteLine($"  {"Origen",-18}   {"Destino",-18} {"Precio",7}   {"Aerolínea",-20} Dur.");
            else
                Console.WriteLine($"  {"Destino",-22} {"Precio",7}   {"Aerolínea",-22} Dur.");
            Console.WriteLine($"  {new string('-', 72)}");
            Console.ResetColor();
        }

        private static void MostrarMenuPrincipal()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║        SISTEMA DE VUELOS BARATOS         ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine();
            Console.WriteLine("  ──────────── REPORTERÍA ──────────────────");
            Console.WriteLine("   1. Ver todas las ciudades");
            Console.WriteLine("   2. Ver vuelos desde una ciudad");
            Console.WriteLine("   3. Ver red completa de vuelos");
            Console.WriteLine("   4. Filtrar vuelos por precio máximo");
            Console.WriteLine("   5. Top 10 vuelos más baratos");
            Console.WriteLine();
            Console.WriteLine("  ──────────── BÚSQUEDA ──────────────");
            Console.WriteLine("   6. Buscar ruta más económica ");
            Console.WriteLine("   7. Buscar vuelos por aerolínea");
            Console.WriteLine(); 
            Console.WriteLine("   8. Salir");
            Console.WriteLine();
            Console.Write("  Opción: ");
        }

        private static void MostrarBienvenida()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║        SISTEMA DE VUELOS BARATOS         ║");
            Console.WriteLine("  ║               BIENVENIDO                 ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("  Presiona cualquier tecla para comenzar...");
            Console.ResetColor();
            Console.ReadKey(intercept: true);
            Console.Clear();
        }

        private static void MostrarDespedida()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  ¡Hasta luego! Buen viaje.\n");
            Console.ResetColor();
        }

        private static void Titulo(string t)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  ── {t} ──");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void Info(string t)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(t);
            Console.ResetColor();
        }

        private static void Alerta(string t)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  ⚠  {t}");
            Console.ResetColor();
        }

        private static void Pausa()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Presiona cualquier tecla para volver al menú...");
            Console.ResetColor();
            Console.ReadKey(intercept: true);
        }
    }
}