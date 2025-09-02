using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Dominio
{
    public class Moto
    {
        public int IdMoto { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Cilindrada { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string FotoUrl { get; set; }
        public short FotoSubida { get; set; }   // SMALLINT → short en C#
        public int IdUsuario { get; set; }

        // Relaciones (opcional, para facilitar)
        public Usuario Usuario { get; set; }
    }
}

