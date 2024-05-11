namespace TesisMarco.DTO
{
	public class EntidadCrear
	{
        public int CodigoEntidad { get; set; }
        public DateTime Vigencia { get; set; }
        public List<PreguntaCrear> Preguntas { get; set; }
    }
}

