using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dominio;
using Negocio;
using System;
using System.ComponentModel.DataAnnotations; // Necesario para [Required]
using System.Linq;

namespace MotoState__.NetCore_Razor.Pages
{
    public class AltaMotoModel : PageModel
    {
        [BindProperty]
        public Moto Moto { get; set; } // Datos de la Moto

        // -------------------------------------------------------------------
        // NUEVAS PROPIEDADES PARA EL SP_RegistrarOrdenCompleta
        // -------------------------------------------------------------------

        [BindProperty]
        [Required(ErrorMessage = "El DNI del Mecánico Principal es obligatorio.")]
        [Display(Name = "DNI Mecánico Principal")]
        public string DniMecanicoPrincipal { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "La descripción de la Orden es obligatoria.")]
        [Display(Name = "Descripción de la Orden")]
        public string DescripcionOrden { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El Servicio Inicial es obligatorio.")]
        [Display(Name = "Servicio Inicial")]
        public string ServicioDesc { get; set; } = "Ingreso y Diagnóstico"; // Default inicial

        [BindProperty]
        [Range(0, 9999999.99, ErrorMessage = "El costo debe ser positivo.")]
        [Display(Name = "Costo Inicial (Estimado)")]
        public decimal CostoServicio { get; set; } = 0.00m; // Costo por defecto

        // Nota: DniMecanicoDetalle lo usaremos igual que DniMecanicoPrincipal por simplicidad inicial.

        public void OnGet()
        {
            // Valores por defecto
            Moto ??= new Moto
            {
                // La fecha de ingreso es mejor dejarla para que el SP use GETDATE()
                // Pero si tu formulario la requiere, la mantenemos
                FechaIngreso = DateTime.Today,
                FotoSubida = 0
            };
        }

        public IActionResult OnPost()
        {
            // Nota: ModelState.IsValid chequeará las validaciones [Required] de Moto y las nuevas propiedades.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var negocio = new MotoNegocio();

                // ?? CAMBIO CRÍTICO: Llama al nuevo método transaccional
                negocio.RegistrarOrdenInicial(
                    Moto.Patente,
                    DniMecanicoPrincipal,
                    DescripcionOrden,
                    ServicioDesc,
                    CostoServicio,
                    DniMecanicoPrincipal // Usamos el mismo DNI para el Mecánico de Detalle
                );

                TempData["Mensaje"] = $"Orden de Reparación y Moto cargada con éxito.";
                return RedirectToPage("MotosEnTaller");
            }
            catch (InvalidOperationException ex)
            {
                // Captura el error del TRIGGER (regla de negocio)
                TempData["Error"] = ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                // Captura otros errores de BD
                TempData["Error"] = $"Error al guardar la orden: {ex.Message}";
                return Page();
            }
        }
    }
}