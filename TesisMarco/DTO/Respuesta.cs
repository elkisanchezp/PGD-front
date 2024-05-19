namespace TesisMarco.DTO
{
	public class Respuesta
	{
        public Guid id { get; set; }
        public bool opcion { get; set; }
        public string? evidencia { get; set; }
        public string? preguntaGEID { get; set; }
        public int formularioFuragId { get; set; }
    }
}

