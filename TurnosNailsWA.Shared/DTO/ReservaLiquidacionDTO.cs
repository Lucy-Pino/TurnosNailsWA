using System;

namespace TurnosNailsWA.Shared.DTO
{
    /// <summary>
    /// Modelo que representa la liquidación económica y distribución de importes de una reserva,
    /// contemplando el 30% de seña original no reembolsable y los descuentos de saldo
    /// correspondientes a servicios cancelados.
    /// </summary>
    public class ReservaLiquidacionDTO
    {
        /// <summary>
        /// Total original de la reserva al momento de su creación.
        /// </summary>
        public decimal TotalOriginal { get; set; }

        /// <summary>
        /// Seña transferida (30% del total original). No admite devolución.
        /// </summary>
        public decimal SeniaTransferida { get; set; }

        /// <summary>
        /// Suma de los precios de lista de los servicios que fueron cancelados.
        /// </summary>
        public decimal TotalCancelados { get; set; }

        /// <summary>
        /// Parte de la seña que corresponde a los servicios cancelados y que el salón retiene (30%).
        /// </summary>
        public decimal SeniaRetenidaCancelados { get; set; }

        /// <summary>
        /// Restante que se descuenta a la clienta por los servicios cancelados (70%).
        /// </summary>
        public decimal RestanteDescontadoCancelados { get; set; }

        /// <summary>
        /// Total correspondiente a los servicios que continúan confirmados/activos.
        /// </summary>
        public decimal TotalActivos { get; set; }

        /// <summary>
        /// Parte de la seña que cubre los servicios activos (30%).
        /// </summary>
        public decimal SeniaActivos { get; set; }

        /// <summary>
        /// Restante a abonar por los servicios activos (70%).
        /// </summary>
        public decimal RestanteActivos { get; set; }

        /// <summary>
        /// Saldo pendiente final a pagar en salón.
        /// </summary>
        public decimal SaldoPendiente { get; set; }

        /// <summary>
        /// Cantidad total de turnos que integraron la reserva original.
        /// </summary>
        public int CantidadTurnosOriginales { get; set; }

        /// <summary>
        /// Cantidad de turnos actualmente cancelados.
        /// </summary>
        public int CantidadTurnosCancelados { get; set; }

        /// <summary>
        /// Cantidad de turnos que siguen activos.
        /// </summary>
        public int CantidadTurnosActivos { get; set; }

        /// <summary>
        /// Indica si la reserva tiene al menos un turno cancelado.
        /// </summary>
        public bool TieneCancelaciones => CantidadTurnosCancelados > 0;
    }
}