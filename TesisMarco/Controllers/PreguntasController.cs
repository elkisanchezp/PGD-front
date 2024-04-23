using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using TesisMarco.DTO;

namespace TesisMarco.Controllers
{
    [Authorize]
    public class PreguntasController : Controller
    {
        public IActionResult Index()
        {

            string apiUrl = "https://pgd-app.onrender.com/api/formularios";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrl);

            // Crear solicitud GET
            var request = new RestRequest("",Method.Get);

            // Ejecutar la solicitud y obtener la respuesta
            var response = client.Execute(request);

            List<Formulario> formularios = new List<Formulario>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                formularios = JsonConvert.DeserializeObject<List<Formulario>>(response.Content);
            }



            return View(formularios);
        }


    }
}
