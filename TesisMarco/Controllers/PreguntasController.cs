using System.Reflection;
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
 

            List<Formulario> formularios = ConsultarFormularios(2);




            return View(formularios);
        }

        public IActionResult CrearFormulario()
        {

            string apiUrlEntidades = "https://pgd-app.onrender.com/api/entidades";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlEntidades);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta
            var response = client.Execute(request);

            List<Entidad> entidades = new List<Entidad>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                entidades = JsonConvert.DeserializeObject<List<Entidad>>(response.Content);
            }

            ViewBag.EntidadUsuario = "No Encontro Entidad";
            ViewBag.IdEntidad = 0;

            if (entidades!= null && entidades.Count> 0)
            {
                var entidadUsuario = entidades.Where(x => x.codigoSigep == 2).FirstOrDefault();
                if(entidadUsuario != null)
                {
                    ViewBag.EntidadUsuario = entidadUsuario.nombre;
                    ViewBag.IdEntidad = entidadUsuario.codigoSigep;
                }
            }

            return PartialView("_CrearFormulario");
        }

        public IActionResult ObtenerGestionExtendida(string codigo)
        {
            string apiUrlEntidades = $"https://pgd-app.onrender.com/api/gestionextendida/preguntasge/{codigo}";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlEntidades);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta
            var response = client.Execute(request);

            List<PreguntaGE> entidades = new List<PreguntaGE>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                entidades = JsonConvert.DeserializeObject<List<PreguntaGE>>(response.Content);
            }

            return PartialView("_GestionExtendida", entidades);

        }


        [HttpPost]

        public JsonResult GuardarFormulario(EntidadCrear entidad)
        {

            try
            {
                string apiUrl = "https://pgd-app.onrender.com/api/formulario";

                // Crear cliente RestSharp
                var client = new RestClient(apiUrl);

                // Crear solicitud POST
                var request = new RestRequest("", Method.Post);
                // ADMIN - NORMAL



                if (entidad?.Preguntas?.Count > 0)
                {
                    int consecutivo = 1;

                    foreach (var item in entidad.Preguntas)
                    {
                        item.Id = "GDI" + consecutivo;
                        consecutivo++;
                    }
                }

                request.AddJsonBody(entidad);

                // Ejecutar la solicitud y obtener la respuesta
                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    return Json(new { Ok = true });
                }
                else
                {
                    return Json(new { Ok = false, Mensaje= response.StatusDescription });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Ok = false, Mensaje = ex.Message });
            }

        }

        [HttpPost]
        public JsonResult EliminarFormulario(int id)
        {
            string apiUrl = $"https://pgd-app.onrender.com/api/formulario/{id}";

            try
            {
                // Crear cliente RestSharp
                var client = new RestClient(apiUrl);

                // Crear solicitud POST
                var request = new RestRequest("", Method.Delete);

                var response = client.Execute(request);


                if (response.IsSuccessful)
                {
                    return Json(new { Ok = true });
                }
                else
                {
                    return Json(new { Ok = false, Mensaje = response.StatusDescription });
                }
            }
            catch(Exception ex)
            {
                return Json(new { Ok = false, Mensaje = ex.Message });
            }

        }


        [HttpPost]

        public JsonResult GuardarPreguntaExtendida(PreguntaGE entidad, string id)
        {

            try
            {

                string apiUrl = $"https://pgd-app.onrender.com/api/gestionextendida/preguntage/{id}";

                // Crear cliente RestSharp
                var client = new RestClient(apiUrl);

                var request = new RestRequest("", Method.Post);

                entidad.Id = id;


                request.AddJsonBody(entidad);

                // Ejecutar la solicitud y obtener la respuesta
                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    return Json(new { Ok = true });
                }
                else
                {
                    return Json(new { Ok = false, Mensaje = response.StatusDescription });
                }

            }
            catch (Exception ex)
            {
                return Json(new { Ok = false, Mensaje = ex.Message });
            }

        }


        [HttpPost]
        public JsonResult EliminarGE(string codigo)
        {
            string apiUrl = $"https://pgd-app.onrender.com/api/gestionextendida/preguntasge/{codigo}";

            try
            {
                // Crear cliente RestSharp
                var client = new RestClient(apiUrl);

                // Crear solicitud POST
                var request = new RestRequest("", Method.Delete);

                var response = client.Execute(request);


                if (response.IsSuccessful)
                {
                    return Json(new { Ok = true });
                }
                else
                {
                    return Json(new { Ok = false, Mensaje = response.StatusDescription });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Ok = false, Mensaje = ex.Message });
            }

        }


        private List<Formulario> ConsultarFormularios(int idEntidad)
        {
            //ajustar y enviar el id de la entidad del logueo

            string apiUrlFormularios = $"https://pgd-app.onrender.com/api/formularios/{idEntidad}";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlFormularios);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta
            var response = client.Execute(request);

            List<Formulario> formularios = new List<Formulario>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                formularios = JsonConvert.DeserializeObject<List<Formulario>>(response.Content);
            }

            return formularios;
        }


    }
}
