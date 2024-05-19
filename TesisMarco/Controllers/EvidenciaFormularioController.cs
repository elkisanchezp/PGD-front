using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using TesisMarco.DTO;
using TesisMarco.Models;

namespace TesisMarco.Controllers
{
    [Authorize]
    public class EvidenciaFormularioController : Controller
    {
        public async Task<IActionResult> Index()
        {
            int idEntidad = 0;

            var idEntidadClaim = User.FindFirst("idEntidad")?.Value;
            if (idEntidadClaim != null)
                idEntidad = int.Parse(idEntidadClaim);

            var formularios = await ConsultarFormulariosAsync(idEntidad);
            return View(formularios.Distinct().ToList());
        }

        private async Task<List<FormularioCompuesto>> ConsultarFormulariosAsync(int idEntidad)
        {
            string apiUrlFormularios = $"https://pgd-app.onrender.com/api/formularios/{idEntidad}";
            var client = new RestClient(apiUrlFormularios);
            var request = new RestRequest("", Method.Get);
            var response = await client.ExecuteAsync(request);

            List<FormularioCompuesto> formularios = new List<FormularioCompuesto>();

            if (response.IsSuccessful)
            {
                formularios = JsonConvert.DeserializeObject<List<FormularioCompuesto>>(response.Content);

                if (formularios != null && formularios.Any())
                {
                    var tareasGE = formularios.Select(formulario => ObtenerEvidencia(formulario));
                    await Task.WhenAll(tareasGE);
                }
            }

            return formularios ?? new List<FormularioCompuesto>();
        }

        private async Task ObtenerEvidencia(FormularioCompuesto formulario)
        {
            string apiUrl = $"https://pgd-app.onrender.com/api/evidencias/formulario/{formulario.Id}";
            var client = new RestClient(apiUrl);
            var request = new RestRequest("", Method.Get);
            var response = await client.ExecuteAsync(request);

            List<Evidencia> entidades = new List<Evidencia>();

            if (response.IsSuccessful)
            {
                entidades = JsonConvert.DeserializeObject<List<Evidencia>>(response.Content);
            }

            formulario.Evidencias = entidades != null && entidades.Any() ? entidades : new List<Evidencia>();
        }
    }
}
