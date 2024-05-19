using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using TesisMarco.DTO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TesisMarco.Controllers
{
    [Authorize]
    public class ReporteGEController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            int idEntidad = 0;

            var idEntidadClaim = User.FindFirst("idEntidad")?.Value;
            if (idEntidadClaim != null)
                idEntidad = int.Parse(idEntidadClaim);           


            List<Formulario> formularios = await Task.Run(() => ConsultarFormulariosAsync(idEntidad));
            return View(formularios.Distinct().ToList());
        }



        public ActionResult GenerarArchivoFURAG(int formularioId)
        {
            var client = new RestClient("https://pgd-app.onrender.com");
            var request = new RestRequest($"/api/formulario/generar/{formularioId}", Method.Post);

            // Puedes agregar encabezados, autenticación u otros parámetros según sea necesario
            // request.AddHeader("Authorization", "Bearer YourAccessToken");

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var archivo = response.RawBytes;

                // Obtener el nombre del archivo de la respuesta del API
                var nombreArchivo = response.ContentHeaders.FirstOrDefault(h => h.Name == "Content-Disposition")?.Value.ToString()
                    .Split(';')
                    .FirstOrDefault(s => s.Trim().StartsWith("filename="))
                    ?.Split('=')[1]
                    .Trim('"');

                if (string.IsNullOrEmpty(nombreArchivo))
                {
                    nombreArchivo = "archivo_furag.xlsx"; // Nombre predeterminado si no se encuentra en la respuesta
                }

                // Devolver el archivo con el nombre y tipo MIME adecuados
                return File(archivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }




        private async Task<List<Formulario>> ConsultarFormulariosAsync(int idEntidad)
        {
            // Ajustar y enviar el id de la entidad del logueo

            string apiUrlFormularios = $"https://pgd-app.onrender.com/api/formularios/{idEntidad}";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlFormularios);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta de forma asíncrona
            var response = await client.ExecuteAsync(request);

            List<Formulario> formularios = new List<Formulario>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                formularios = JsonConvert.DeserializeObject<List<Formulario>>(response.Content);

                if (formularios != null && formularios.Count > 0)
                {
                    // Lista para almacenar todas las tareas de obtención de preguntas GE
                    List<Task> tareasGE = new List<Task>();

                    foreach (var formulario in formularios.Distinct().ToList())
                    {
                        foreach (var pregunta in formulario.Preguntas.Distinct().ToList())
                        {
                            // Agregar la tarea de obtención de preguntas GE a la lista
                            tareasGE.Add(ObtenerPreguntasGEAsync(pregunta, formulario.Id));
                        }
                    }

                    // Esperar a que todas las tareas de obtención de preguntas GE se completen
                    await Task.WhenAll(tareasGE);
                }
            }

            return formularios.Distinct().ToList();
        }

        private async Task<List<PreguntaGE>> ObtenerPreguntasGEAsync(Pregunta pregunta, int idFormulario)
        {
            string apiUrlEntidades = $"https://pgd-app.onrender.com/api/gestionextendida/preguntasge/{pregunta.Id}";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlEntidades);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta de forma asíncrona
            var response = await client.ExecuteAsync(request);

            List<PreguntaGE> entidades = new List<PreguntaGE>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                entidades = JsonConvert.DeserializeObject<List<PreguntaGE>>(response.Content);
            }

            pregunta.PreguntasGE = entidades ?? new List<PreguntaGE>();
            var preguntasGE =  pregunta.PreguntasGE.Distinct().ToList();

            if (preguntasGE != null && preguntasGE.Count > 0)
            {
                // Lista para almacenar todas las tareas de obtención de preguntas GE
                List<Task> tareasRespuestaGE = new List<Task>();

                foreach (var item in preguntasGE.Distinct().ToList())
                {
                    tareasRespuestaGE.Add(ObtenerRespuestaGEAsync(item, idFormulario));
                }

                // Esperar a que todas las tareas de obtención de preguntas GE se completen
                await Task.WhenAll(tareasRespuestaGE);
            }

            return preguntasGE;
        }



        private async Task<List<Respuesta>> ObtenerRespuestaGEAsync(PreguntaGE pregunta, int idFormulario)
        {
            string apiUrl = $"https://pgd-app.onrender.com/api/gestionextendida/respuestasge/{pregunta.Id}/{idFormulario}";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrl);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta de forma asíncrona
            var response = await client.ExecuteAsync(request);

            List<Respuesta> entidades = new List<Respuesta>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                entidades = JsonConvert.DeserializeObject<List<Respuesta>>(response.Content);
            }

            pregunta.Respuestas = entidades ?? new List<Respuesta>();
            var respuestasGE = pregunta.Respuestas.Distinct().ToList();

            return respuestasGE;
        }




    }
}

