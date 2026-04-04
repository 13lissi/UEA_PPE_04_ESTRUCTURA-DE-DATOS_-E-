# 🛫 Sistema de Búsqueda de Vuelos Baratos

## 1. Información General
* **Institución:** Universidad Estatal Amazónica (UEA)
* **Carrera:** Tecnologías de la Información
* **Asignatura:** Estructura de Datos
* **Autores:** Garcia Arreaga Jeison Teobaldo, Cornejo Olaya Lissi Antonella
* **Proyecto:** Sistema de Búsqueda de Vuelos Baratos
* **Estructuras de datos utilizadas:** Grafo dirigido (Lista de adyacencia), HashSet, Dictionary, PriorityQueue y Algoritmo de Dijkstra.

## 2. Descripción del Proyecto
Es una aplicación de consola desarrollada en C# que permite encontrar la ruta aérea más económica entre dos ciudades. Utiliza un grafo dirigido ponderado para modelar las conexiones y el algoritmo de Dijkstra para calcular el menor costo total. Cuenta con una red de **84 rutas** aéreas distribuidas en Ecuador, Sudamérica, Norte/Centroamérica y Europa, cargadas dinámicamente desde un archivo `vuelos.txt`.

## 3. Arquitectura del Sistema
El proyecto aplica el Principio de Responsabilidad Única (SRP), separando la lógica en clases específicas:
* **`Vuelo.cs` / `ResultadoRuta.cs`:** Modelos de datos (aristas del grafo y resultados).
* **`GrafoVuelos.cs`:** Implementa la lógica del grafo y el algoritmo de Dijkstra.
* **`CargadorVuelos.cs`:** Parsea la base de datos de vuelos con sistema de respaldo automático.
* **`Normalizador.cs`:** Limpia las entradas del usuario (manejo de alias y mayúsculas).
* **`ControladorVuelos.cs` / `ConsolaUI.cs`:** Coordinan la lógica interactiva y presentan el menú al usuario.

## 4. Análisis de las Estructuras de Datos Utilizadas
* **Grafo dirigido con lista de adyacencia (`Dictionary<string, List<Vuelo>>`):** Se eligió sobre la matriz de adyacencia por ser óptima en memoria `O(V + E)` para redes dispersas.
* **`Dictionary` y `HashSet`:** Permiten acceso, inserción y validación de ciudades sin duplicados en tiempo promedio `O(1)`. El uso de `StringComparer.OrdinalIgnoreCase` evita errores de tipeo por mayúsculas/minúsculas.
* **Algoritmo de Dijkstra con `PriorityQueue`:** Implementa un min-heap nativo de .NET, permitiendo extraer el nodo de menor costo acumulado en `O(log V)`, logrando cálculos de rutas en menos de 1 milisegundo.

## 5. Conclusión
La elección del grafo con lista de adyacencia y el algoritmo de Dijkstra resultó idónea y eficiente para modelar esta red de vuelos. La integración de estructuras de datos nativas de C# garantizó una complejidad óptima y un código limpio. La separación de responsabilidades hace que el sistema sea escalable, facilitando la adición de nuevas rutas o funcionalidades.

---
<div align="center">
  <p>Proyecto académico desarrollado para la asignatura de Estructura de Datos</p>
  <p>Carrera de Tecnologías de la Información</p>
  <p>Universidad Estatal Amazónica · 2026</p>
</div>
