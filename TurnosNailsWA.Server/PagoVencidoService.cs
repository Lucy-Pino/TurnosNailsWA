using TurnosNailsWA.Repositorio.Repositorios;

namespace TurnosNailsWA.Server
{
    // Revisa cada minuto si algún pago pendiente superó los 30 minutos sin verificar.
    public class PagoVencidoService : BackgroundService
    {
        private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(1);
        private const int MinutosVencimiento = 30;

        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<PagoVencidoService> logger;

        public PagoVencidoService(IServiceScopeFactory scopeFactory, ILogger<PagoVencidoService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var temporizador = new PeriodicTimer(Intervalo);

            try
            {
                do
                {
                    try
                    {
                        using var scope = scopeFactory.CreateScope();
                        var pagoRepositorio = scope.ServiceProvider.GetRequiredService<IPagoRepositorio>();
                        var actualizados = await pagoRepositorio.ActualizarVencidos(MinutosVencimiento);
                        if (actualizados > 0)
                        {
                            logger.LogInformation("Pagos procesados y actualizados por el servicio de vencimiento: {Cantidad}.", actualizados);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Un error de esta vuelta no debe detener el servicio.
                        logger.LogError(ex, "Error al actualizar los pagos vencidos.");
                    }
                }
                while (await temporizador.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
                // Cierre normal de la aplicación.
            }
        }
    }
}
