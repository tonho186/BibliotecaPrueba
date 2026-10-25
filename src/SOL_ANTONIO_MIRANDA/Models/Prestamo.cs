namespace SOL_ANTONIO_MIRANDA.Models
{
    public class Prestamo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string ISBN { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string TipoLectura { get; set; } // "Sala" o "Casa"
        public bool Devuelto { get; set; }

        public Usuario Usuario { get; set; }
        public Libro Libro { get; set; }
    }
}
