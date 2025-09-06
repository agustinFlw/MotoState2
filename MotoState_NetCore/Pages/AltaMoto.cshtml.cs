using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dominio; // Importa tu clase Moto
using System;

namespace MotoState__.NetCore_Razor.Pages
{
    public class AltaMotoModel : PageModel
    {
        [BindProperty]
        public Moto Moto { get; set; }

        public void OnGet()
        {
            // Si necesitas inicializar algo
            Moto = new Moto
            {
                FechaIngreso = DateTime.Now,
                FotoSubida = 0
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // TODO: Guardar en la base de datos
            // Ejemplo: _context.Motos.Add(Moto); _context.SaveChanges();

            return RedirectToPage("MotosEnTaller");
        }
    }
}
