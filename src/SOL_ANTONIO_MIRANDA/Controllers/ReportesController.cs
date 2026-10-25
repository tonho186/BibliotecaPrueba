using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOL_ANTONIO_MIRANDA.Data;

namespace SOL_ANTONIO_MIRANDA.Controllers
{
    public class ReportesController : Controller
    {
        private readonly BibliotecaContext _context;

        public ReportesController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: /Reportes/
        public async Task<IActionResult> Index()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();

            return View(prestamos);
        }

        [HttpPost]
        public async Task<IActionResult> Devolver(int prestamoId)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == prestamoId);

            if (prestamo == null)
                return NotFound();

            if (!prestamo.Devuelto)
            {
                prestamo.Devuelto = true;

                if (DateTime.Now > prestamo.FechaDevolucion)
                {
                    prestamo.Usuario.Estado = false; // Sancionar usuario
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
