using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using TesisMarco.DTO;
using TesisMarco.Models;

namespace TesisMarco.Controllers
{
    [Authorize]
    public class SistemaPreguntasController : Controller
    {
        public async Task<IActionResult> Index()
        {
            int idEntidad = 2;
            // Recuperar el idEntidad del claim
            var idEntidadClaim = User.FindFirst("idEntidad")?.Value;
            if (idEntidadClaim != null)
            {
                idEntidad = int.Parse(idEntidadClaim);
                // Ahora puedes utilizar el idEntidad en este controlador
            }


            List<Formulario> formularios = await Task.Run(() => ConsultarFormulariosAsync(idEntidad));
            return View(formularios.Distinct().ToList());
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
                            tareasGE.Add(ObtenerPreguntasGEAsync(pregunta));
                        }
                    }

                    // Esperar a que todas las tareas de obtención de preguntas GE se completen
                    await Task.WhenAll(tareasGE);
                }
            }

            return formularios.Distinct().ToList();
        }



        private async Task<List<PreguntaGE>> ObtenerPreguntasGEAsync(Pregunta pregunta)
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
            return pregunta.PreguntasGE.Distinct().ToList();
        }


        [HttpPost]

        public JsonResult GuardarRespuestas(List<GuardarRespuesta> formularios)
        {

            try
            {
                foreach (var item in formularios)
                    GuardarRespuesta(new GuardarRespuestaApi { evidencia = item.Evidencia, opcion = item.Opcion, preguntaGEID = item.PreguntaGEID }, item.IdFurmulario, item.PreguntaGEID);
                
                return Json(new { Ok = true });
            }
            catch (Exception ex)
            {
                return Json(new { Ok = false, Mensaje = ex.Message });
            }

        }

        [HttpPost]
        public ActionResult AnalizarEvidencia(int formularioId, int preguntaId, string enunciado, string evidencia)
        {
            // Realiza el análisis de la evidencia aquí
            // Puedes agregar tu lógica de análisis de evidencia

            string apiUrlEvidencias = $"https://pgd-analisis-evidencias.onrender.com/analitica/evidencias";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlEvidencias);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Post);

            var entidades = ConsultarEntidades();

            string entidad = "";

            if (entidades != null && entidades.Count > 0)
            {
                var entidadsigep = entidades.Where(x => x.codigoSigep == 2).FirstOrDefault();
                entidad = entidadsigep == null ? "": entidadsigep.nombre;

            }



            request.AddJsonBody(new { pregunta_ge = enunciado, evidencia = evidencia, entidad = entidad });

            // Ejecutar la solicitud y obtener la respuesta de forma asíncrona
            var response = client.Execute(request);

            List<string> mensajes = new List<string>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                mensajes = JsonConvert.DeserializeObject<List<string>>(response.Content);
            }
            else
            {
                return Json(new { ok = false, mensaje = response.ErrorMessage});
            }

            if (mensajes != null && mensajes.Count > 0)
            {

                StringBuilder mensajesHomologados = new StringBuilder();

                foreach (var respuesta in mensajes)
                {
                    // Concatenamos el tipo y la descripción del mensaje
                    string mensaje = "";

                    switch (respuesta)
                    {
                        case "NO_ALERTAS":
                            mensaje = "No hay alertas: No se detectó ninguna señal de alerta en la evidencia analizada.";
                            break;
                        case "NO_LINK":
                            mensaje = "No hay link: La evidencia analizada no constituye un link o URL para acceder a un repositorio o sitio web con evidencias o documentos.";
                            break;
                        case "TEXTO_GENERICO":
                            mensaje = "Texto genérico: La evidencia analizada, además de ser texto plano, no guarda ninguna relación con la pregunta de gestión extendida para la cual debe dar soportes.";
                            break;
                        case "LINK_MALO":
                            mensaje = "Link malo: La evidencia analizada constituye un link, pero el mismo no responde con estado 200 OK por estar mal formado, apuntar a una red local, necesitar autenticación, etc.";
                            break;
                        case "LINK_NO_RELACION_ENTIDAD":
                            mensaje = "Link sin relación a la entidad: El link analizado direcciona a una página web que posiblemente no guarda ninguna relación con la entidad para la cual se está haciendo el análisis de evidencias.";
                            break;
                        case "LINK_NO_RELACION_FURAG":
                            mensaje = "Link sin relación a FURAG: El link analizado direcciona a una página web que posiblemente no contiene la palabra 'FURAG'.";
                            break;
                        case "LINK_NO_ARCHIVOS":
                            mensaje = "Link sin archivos: El link analizado direcciona a una página web que posiblemente no contiene ninguna lista de archivos, links, u otros elementos que puedan servir como evidencias para la pregunta de gestión extendida referenciada.";
                            break;
                        default:
                            mensaje = "Respuesta no reconocida: La respuesta recibida no se reconoce como un tipo de mensaje homologado.";
                            break;
                    }

                    // Agregamos el mensaje a la lista
                    mensajesHomologados.AppendLine(mensaje);

                }




                // Por ejemplo, aquí simplemente se devuelve un mensaje de éxito
                return Json(new { ok = true, mensaje = mensajesHomologados.ToString() });
            }
            else
            {
                return Json(new { ok = true, mensaje = "Sin mensaje" });
            }


        }




        private void GuardarRespuesta(GuardarRespuestaApi modelo, string idFormulario, string preguntaId)
        {
            string apiUrl = $"https://pgd-app.onrender.com/api/gestionextendida/respuestasge/{idFormulario}/{preguntaId}";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrl);

            var request = new RestRequest("", Method.Post);

            request.AddJsonBody(modelo);

            var response = client.Execute(request);

        }

        private List<Entidad> ConsultarEntidades()
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

            return entidades;
        }





    }


}