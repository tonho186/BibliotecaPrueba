namespace SOL_ANTONIO_MIRANDA.Models
{
    public class TipoUsuario
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
