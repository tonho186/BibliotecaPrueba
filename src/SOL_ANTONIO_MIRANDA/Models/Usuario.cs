namespace SOL_ANTONIO_MIRANDA.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Dni { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string TipoUsuario { get; set; } // "Alumno" o "Docente"
        public bool Estado { get; set; } // true = Habilitado, false = Sancionado

        public ICollection<Prestamo> Prestamos { get; set; }
    }
}
