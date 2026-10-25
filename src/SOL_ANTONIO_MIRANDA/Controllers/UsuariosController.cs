using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOL_ANTONIO_MIRANDA.Data;

namespace SOL_ANTONIO_MIRANDA.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BibliotecaContext _context;

        public UsuariosController(BibliotecaContext context)
        {
            _context = context;
        }

        // Lista de usuarios sancionados
        public async Task<IActionResult> Sancionados()
        {
            var sancionados = await _context.Usuarios
                .Where(u => u.Estado == false)
                .ToListAsync();

            return View(sancionados);
        }

        // Acción para rehabilitar
        [HttpPost]
        public async Task<IActionResult> Rehabilitar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Estado = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Sancionados");
        }

        public async Task<IActionResult> Perfil()
        {
            int usuarioId = ObtenerUsuarioActualId();

            var usuario = await _context.Usuarios
                .Include(u => u.Prestamos)
                    .ThenInclude(p => p.Libro)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                return NotFound();

            var hoy = DateTime.Today;

            int prestamosHoy = usuario.Prestamos.Count(p => p.FechaPrestamo.Date == hoy);
            int librosSinDevolver = usuario.Prestamos.Count(p => !p.Devuelto);

            ViewBag.PrestamosHoy = prestamosHoy;
            ViewBag.LibrosSinDevolver = librosSinDevolver;

            return View(usuario);
        }

        private int ObtenerUsuarioActualId()
        {
            return int.Parse(User.FindFirst("UsuarioId").Value);
        }
    }
}
