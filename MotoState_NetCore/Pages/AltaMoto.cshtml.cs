using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dominio;
using Negocio; // <� Importante
using System;

namespace MotoState__.NetCore_Razor.Pages
{
    public class AltaMotoModel : PageModel
    {
        [BindProperty]
        public Moto Moto { get; set; }

        public void OnGet()
        {
            // Valores por defecto
            Moto ??= new Moto
            {
                FechaIngreso = DateTime.Today,
                FotoSubida = 0
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Esta secci�n nos dir� qu� est� mal.
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();

                // Pon un breakpoint en la l�nea de abajo para ver la variable "errors".
                return Page();
            }

            // Si llega ac�, es porque el modelo es v�lido.
            try
            {
                var negocio = new MotoNegocio();
                int nuevoId = negocio.Agregar(Moto);

                TempData["Mensaje"] = $"Moto #{nuevoId} cargada con �xito.";
                return RedirectToPage("MotosEnTaller");
            }
            catch (Exception ex)
            {
                // Es buena pr�ctica tener un manejo de errores aqu� tambi�n.
                TempData["Error"] = $"Error al guardar la moto: {ex.Message}";
                return Page();
            }
        }
    }
}
