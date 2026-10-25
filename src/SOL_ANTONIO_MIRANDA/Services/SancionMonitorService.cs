using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SOL_ANTONIO_MIRANDA.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SOL_ANTONIO_MIRANDA.Services
{
    public class SancionMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SancionMonitorService> _logger;

        public SancionMonitorService(IServiceProvider serviceProvider, ILogger<SancionMonitorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<BibliotecaContext>();

                    var vencidos = db.Prestamos
                        .Where(p => !p.Devuelto && p.FechaDevolucion < DateTime.Now)
                        .ToList();

                    foreach (var prestamo in vencidos)
                    {
                        var usuario = db.Usuarios.FirstOrDefault(u => u.Id == prestamo.UsuarioId);
                        if (usuario != null && usuario.Estado)
                        {
                            usuario.Estado = false;
                            _logger.LogInformation($"Usuario {usuario.Nombres} sancionado por retraso en préstamo {prestamo.Id}");
                        }
                    }

                    await db.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Revisión cada 30 segundos
            }
        }
    }
}
