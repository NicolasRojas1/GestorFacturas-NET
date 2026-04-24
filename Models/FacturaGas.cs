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

    public Gas() {}

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

    public (int c1, int c2) CalcularConsumos()
    {
        int consumo1 = ConsumoFinal - ConsumoInicial;
        int consumo2 = ConsumoTotal - consumo1;
        return (consumo1, consumo2);
    }

    public (decimal v1, decimal v2) CalcularPorcentajes()
    {
        if (ConsumoTotal == 0)
        {
            return (50, 50);
        }
        var consumos = CalcularConsumos();
        decimal consumo1 = consumos.c1;
        decimal consumo2 = consumos.c2;
        decimal porcentaje1 = consumo1 / ConsumoTotal * 100m;
        decimal porcentaje2 = consumo2 / ConsumoTotal * 100m;
        return (porcentaje1, porcentaje2);
    }

    public (decimal total1, decimal total2) CalcularPagos()
    {
        var porcentajes = CalcularPorcentajes();
        decimal cargoPorApto = CargoFijo / 2m;

        decimal consumoVariableTotal = ValorRecibo - CargoFijo;

        if (porcentajes.v1 == 0 && porcentajes.v2 == 0)
        {
            return (Math.Round(cargoPorApto, 0), Math.Round(cargoPorApto, 0));
        }

        decimal pagoVariable1 = (porcentajes.v1 / 100) * consumoVariableTotal;
        decimal pagoVariable2 = (porcentajes.v2 / 100) * consumoVariableTotal;

        decimal total1 = cargoPorApto + pagoVariable1;
        decimal total2 = cargoPorApto + pagoVariable2;

        return (Math.Round(total1, 0), Math.Round(total2, 0));
    }
    public override string ObtenerReporte()
    {
        var (c1, c2) = CalcularConsumos();
        var (p1, p2) = CalcularPorcentajes();
        var (total1, total2) = CalcularPagos();

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
   Consumo:      {c1} m3 ({p1:F1}%)
   VALOR A PAGAR: {total1:C0}

APTO 2:
   Consumo:      {c2} m3 ({p2:F1}%)
   VALOR A PAGAR: {total2:C0}
--------------------------------------------------
    *Cálculos basados en el consumo real del medidor*
==================================================";
    }
}

