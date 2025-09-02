using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Dominio
{
    public class Seguimiento
    {
        public int IdSeguimiento { get; set; }
        public int IdMoto { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string Diagnostico { get; set; }

        // Relaciones (opcional)
        public Moto Moto { get; set; }
        public Estado Estado { get; set; }
    }
}

