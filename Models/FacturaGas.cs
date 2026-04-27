using System.ComponentModel.DataAnnotations;

namespace AppFacturas.Models;

public class Gas : Factura
{
    [Required]
    [Range(0, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    public int ConsumoInicial { get; set; }

    [Required]
    [Range(0, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    public int ConsumoFinal { get; set; }

    [Required]
    [Range(0.01, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    [DataType(DataType.Currency)]
    public decimal CargoFijo { get; set; }
    public int ConsumoTotal => LecturaFinalRecibo - LecturaInicialRecibo;
    public int ConsumoApto1 { get; set; }
    public int ConsumoApto2 { get; set; }
    public decimal PorcentajeApto1 { get; set; }
    public decimal PorcentajeApto2 { get; set; }
    public decimal CargoPorApto => CargoFijo / 2m;
    public decimal ValorConsumoNeto => ValorRecibo - CargoFijo; // Calculo sin el cargo fijo                               
    public decimal TotalApto1 { get; set; }
    public decimal TotalApto2 { get; set; }
    public decimal ValorConsumoApto1 => TotalApto1 - CargoPorApto; // Lo que paga el apto 1 solo por lo que consumió 
    public decimal ValorConsumoApto2 => TotalApto2 - CargoPorApto; // Lo que paga el apto 2 solo por lo que consumió 

    public Gas() { }

    public Gas(decimal valorRecibo, int lecturaInicialRecibo, int lecturaFinalRecibo, DateTime fechaInicial, DateTime fechaFinal, int consumoInicial, int consumoFinal, decimal cargoFijo)
    : base(valorRecibo, lecturaInicialRecibo, lecturaFinalRecibo, fechaInicial, fechaFinal)
    {
        ConsumoInicial = consumoInicial;
        ConsumoFinal = consumoFinal;
        CargoFijo = cargoFijo;
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 1. Validaciones de la clase Padre (Fechas, etc.)
        foreach (var error in base.Validate(validationContext))
        {
            yield return error;
        }

        // 2. Agregamos validaciones exclusivas de Gas
        if (ConsumoFinal < ConsumoInicial)
        {
            yield return new ValidationResult(
                "El consumo final del contador no puede ser menor al inicial.",
                new[] { nameof(ConsumoFinal) }
            );
        }
    }
    protected void CalcularConsumos()
    {
        this.ConsumoApto1 = ConsumoFinal - ConsumoInicial;
        this.ConsumoApto2 = ConsumoTotal - this.ConsumoApto1;
    }

    protected void CalcularPorcentajes()
    {
        if (ConsumoTotal == 0)
        {
            this.PorcentajeApto1 = 50;
            this.PorcentajeApto2 = 50;
            return;
        }
        this.PorcentajeApto1 = (decimal)this.ConsumoApto1 / (decimal)ConsumoTotal * 100m;
        this.PorcentajeApto2 = (decimal)this.ConsumoApto2 / (decimal)ConsumoTotal * 100m;
    }

    protected void CalcularPagos()
    {
        // Escenario A: Nadie usó gas, solo se divide el cargo fijo
        if (this.ConsumoApto1 == 0 && this.ConsumoApto2 == 0)
        {
            this.TotalApto1 = Math.Round(this.CargoPorApto, 0);
            this.TotalApto2 = Math.Round(this.CargoPorApto, 0);
            return;
        }
        this.TotalApto1 = Math.Round((this.PorcentajeApto1 / 100 * this.ValorConsumoNeto) + this.CargoPorApto, 0);
        this.TotalApto2 = Math.Round((this.PorcentajeApto2 / 100 * this.ValorConsumoNeto) + this.CargoPorApto, 0);
    }

    public void ProcesarFactura()
    {
        CalcularConsumos();
        CalcularPorcentajes();
        CalcularPagos();
    }

    protected override string ObtenerReporte()
    {
        // Usamos los nombres reales de tus propiedades
        return $@"
==================================================
      🔥 REPORTE DE CONSUMO DE GAS 🔥
==================================================
[ DATOS DEL RECIBO ]
--------------------------------------------------
Periodo:        {FechaInicial:d} al {FechaFinal:d}
Valor Total:    {ValorRecibo:C0}
Cargo Fijo:     {CargoFijo:C0}

Lectura Recibo: {LecturaInicialRecibo} m3 - {LecturaFinalRecibo} m3
Lectura Medidor:{ConsumoInicial} m3 - {ConsumoFinal} m3
--------------------------------------------------

[ DISTRIBUCIÓN POR APARTAMENTO ]
--------------------------------------------------
APTO 1:
   Consumo:      {ConsumoApto1} m3 ({PorcentajeApto1:F1}%)
   VALOR A PAGAR: {TotalApto1:C0}

APTO 2:
   Consumo:      {ConsumoApto2} m3 ({PorcentajeApto2:F1}%)
   VALOR A PAGAR: {TotalApto2:C0}
--------------------------------------------------
    *Cálculos basados en el consumo real del medidor*
==================================================";
    }
}

