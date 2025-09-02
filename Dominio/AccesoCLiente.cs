using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class AccesoCliente
    {
        public int IdAcceso { get; set; }
        public int IdMoto { get; set; }
        public string UrlUnica { get; set; }

        // Relación (opcional)
        public Moto Moto { get; set; }
    }
}

