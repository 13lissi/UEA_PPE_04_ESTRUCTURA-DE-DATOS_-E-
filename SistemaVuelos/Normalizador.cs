namespace SistemaVuelos
{
    // Solo limpia el input del usuario.
    // La búsqueda real (parcial, case-insensitive) la hace ConsolaUI.
    public static class Normalizador
    {
        // Alias cortos → nombre exacto del txt
        private static readonly Dictionary<string, string> Alias =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "gye",         "Guayaquil"        },
                { "uio",         "Quito"            },
                { "ny",          "New York"         },
                { "nyc",         "New York"         },
                { "ba",          "Buenos Aires"     },
                { "baires",      "Buenos Aires"     },
                { "buenosaires", "Buenos Aires"     },
                { "cdmx",        "Ciudad de Mexico" },
                { "mexico",      "Ciudad de Mexico" },
                { "mex",         "Ciudad de Mexico" },
                { "sp",          "Sao Paulo"        },
                { "saopaulo",    "Sao Paulo"        },
                { "la",          "Los Angeles"      },
                { "losangeles",  "Los Angeles"      },
                { "bog",         "Bogota"           },
                { "lim",         "Lima"             },
                { "scl",         "Santiago"         },
                { "mad",         "Madrid"           },
                { "cdg",         "Paris"            },
                { "lhr",         "Londres"          },
                { "ber",         "Berlin"           },
                { "ams",         "Amsterdam"        },
                { "mia",         "Miami"            },
                { "ccs",         "Caracas"          },
                { "eze",         "Buenos Aires"     },
                { "gru",         "Sao Paulo"        },
            };

        /// <summary>
        /// Limpia el input y resuelve alias.
        /// Devuelve el string limpio para que ConsolaUI haga la búsqueda parcial.
        /// </summary>
        public static string Limpiar(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var limpio = input.Trim();

            // Resolver alias (sin espacios, sin tildes, minúsculas)
            var clave = QuitarTildes(limpio).ToLowerInvariant().Replace(" ", "");
            if (Alias.TryGetValue(clave, out var alias))
                return alias;

            return limpio; // devolver tal como el usuario escribió
        }

        private static string QuitarTildes(string s) =>
            s.Replace("á","a").Replace("é","e").Replace("í","i")
             .Replace("ó","o").Replace("ú","u")
             .Replace("Á","A").Replace("É","E").Replace("Í","I")
             .Replace("Ó","O").Replace("Ú","U");
    }
}