using System.ComponentModel.DataAnnotations;

namespace SOL_ANTONIO_MIRANDA.Models
{
    public class Libro
    {
        [Key]
        public string ISBN { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string Autor { get; set; }
        public string Pais { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Editorial { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; }
    }
}
