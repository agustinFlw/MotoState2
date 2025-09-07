using System;
using System.ComponentModel.DataAnnotations;

namespace Dominio
{
    public class Moto
    {
        public int IdMoto { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio")]
        public string Modelo { get; set; }

        public string Patente { get; set; }

        public DateTime FechaIngreso { get; set; }

        public string FotoUrl { get; set; }

        public short FotoSubida { get; set; }   // SMALLINT → short en C#

        [Required(ErrorMessage = "El ID de usuario es obligatorio")]
        public int? IdUsuario { get; set; }

        
    }
}
