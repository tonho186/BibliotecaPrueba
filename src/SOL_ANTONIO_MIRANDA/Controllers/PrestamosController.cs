using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SOL_ANTONIO_MIRANDA.Data;
using SOL_ANTONIO_MIRANDA.Models;
using System.Linq;

namespace SOL_ANTONIO_MIRANDA.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly BibliotecaContext _context;

        public PrestamosController(BibliotecaContext context)
        {
            _context = context;
        }

        // Registrar préstamo
        [HttpPost]
        public async Task<IActionResult> Registrar(int usuarioId, string isbn, string tipoLectura)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (!usuario.Estado)
                return BadRequest("Usuario sancionado.");

            var hoy = DateTime.Today;
            var prestamosHoy = _context.Prestamos
                .Where(p => p.UsuarioId == usuarioId && p.FechaPrestamo.Date == hoy).Count();

            if (prestamosHoy >= 3)
                return BadRequest("Máximo 3 préstamos por día.");

            var prestamo = new Prestamo
            {
                UsuarioId = usuarioId,
                ISBN = isbn,
                TipoLectura = tipoLectura,
                FechaPrestamo = DateTime.Now,
                FechaDevolucion = DateTime.Now.AddMinutes(1), // Simulación
                Devuelto = false
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            return Ok("Préstamo registrado.");
        }

        // Devolver préstamo
        [HttpPost]
        public async Task<IActionResult> Devolver(int prestamoId)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == prestamoId);

            if (prestamo == null) return NotFound();

            prestamo.Devuelto = true;
            if (DateTime.Now > prestamo.FechaDevolucion)
            {
                prestamo.Usuario.Estado = false; // Sancionar
            }

            await _context.SaveChangesAsync();
            return Ok("Devolución registrada.");
        }

        // GET: Prestamos/Create
        public IActionResult Create()
        {
            var usuariosHabilitados = _context.Usuarios.Where(u => u.Estado == true).ToList();
            ViewBag.Usuarios = new SelectList(usuariosHabilitados, "Id", "Nombres");

            var librosPrestados = _context.Prestamos
                .Where(p => !p.Devuelto)
                .Select(p => p.ISBN)
                .ToList();

            var librosDisponibles = _context.Libros
                .Where(l => !librosPrestados.Contains(l.ISBN))
                .ToList();

            ViewBag.Libros = new SelectList(librosDisponibles, "ISBN", "Nombre");
            ViewBag.UsuariosSancionados = _context.Usuarios.Where(u => !u.Estado).ToList();

            return View();
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,ISBN,TipoLectura")] Prestamo prestamo)
        {
            int usuarioId = ObtenerUsuarioActualId();
            var usuario = await _context.Usuarios.Include(u => u.Prestamos).FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario.Prestamos.Any(p => !p.Devuelto))
            {
                TempData["Error"] = "No puedes realizar nuevos préstamos hasta devolver los libros pendientes.";
                return RedirectToAction("Perfil", "Usuarios");
            }

            usuario = await _context.Usuarios.FindAsync(prestamo.UsuarioId);
            if (usuario == null || !usuario.Estado)
            {
                ModelState.AddModelError("", "El usuario no existe o está sancionado.");
            }

            var prestamosHoy = await _context.Prestamos
                .Where(p => p.UsuarioId == prestamo.UsuarioId && p.FechaPrestamo.Date == DateTime.Today)
                .CountAsync();

            if (prestamosHoy >= 3)
            {
                ModelState.AddModelError("", "Máximo 3 préstamos por día.");
            }

            if (ModelState.IsValid)
            {
                if (await _context.Prestamos.AnyAsync(p => p.ISBN == prestamo.ISBN && !p.Devuelto))
                {
                    ModelState.AddModelError("", "El libro ya está prestado y aún no ha sido devuelto.");
                }
                else
                {
                    prestamo.FechaPrestamo = DateTime.Now;
                    prestamo.FechaDevolucion = DateTime.Now.AddMinutes(1); // Simulación
                    prestamo.Devuelto = false;

                    _context.Add(prestamo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Reportes");
                }
            }

            ViewBag.Usuarios = new SelectList(_context.Usuarios.Where(u => u.Estado == true), "Id", "Nombres", prestamo.UsuarioId);
            ViewBag.Libros = new SelectList(_context.Libros, "ISBN", "Nombre", prestamo.ISBN);

            return View(prestamo);
        }
        private int ObtenerUsuarioActualId()
        {
            return int.Parse(User.FindFirst("UsuarioId").Value);
        }
    }
}
