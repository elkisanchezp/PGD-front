using System;
namespace TesisMarco.DTO
{
	public class EntidadPuntaje
	{
        public string nombreEntidad { get; set; }
        public int codigosigep { get; set; }
        public List<Puntaje> puntajes { get; set; }
    }
}

