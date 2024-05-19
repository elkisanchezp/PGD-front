namespace TesisMarco.DTO
{
	public class PreguntaGE
	{
        public string Id { get; set; }
        public string Enunciado { get; set; }
        public string EvidenciaSugerida { get; set; }
        public string RolSugerido { get; set; }
        public string IdPreguntaFurag { get; set; }
        public List<Respuesta>? Respuestas { get; set; }

    }
}

