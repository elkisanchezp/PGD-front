namespace TesisMarco.DTO
{
    public class Formulario
    {
        public int Id { get; set; }
        public DateTime Vigencia { get; set; }
        public List<Pregunta> Preguntas { get; set; }
    }
}
