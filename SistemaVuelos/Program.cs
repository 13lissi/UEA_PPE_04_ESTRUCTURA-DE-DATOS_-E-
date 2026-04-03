namespace SistemaVuelos
{
    class Program
    {
        static void Main()
        {
            var controlador = new ControladorVuelos();
            var ui = new ConsolaUI(controlador);
            ui.Iniciar();
        }
    }
}