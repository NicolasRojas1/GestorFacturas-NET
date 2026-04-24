using System.ComponentModel.DataAnnotations;

namespace AppFacturas.Models;

public class Luz : Factura
{
    [Required]
    [Range(0, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    public int ConsumoInicial { get; set; }

    [Required]
    [Range(0, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    public int ConsumoFinal { get; set; }

    [Required]
    [Range(0.0001, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    [DataType(DataType.Currency)]
    public decimal ValorKw { get; set; }

    [Required]
    [Range(0.01, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    [DataType(DataType.Currency)]
    public decimal ValorAseo { get; set; }
    public int ConsumoTotal => LecturaFinalRecibo - LecturaInicialRecibo;

    public Luz() { }

    public Luz(decimal valorRecibo, int lecturaInicialRecibo, int lecturaFinalRecibo, DateTime fechaInicial, DateTime fechaFinal, int consumoInicial, int consumoFinal, decimal valorKw, decimal valorAseo) : base(valorRecibo, lecturaInicialRecibo, lecturaFinalRecibo, fechaInicial, fechaFinal)
    {
        ConsumoInicial = consumoInicial;
        ConsumoFinal = consumoFinal;
        ValorKw = valorKw;
        ValorAseo = valorAseo;
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var error in base.Validate(validationContext))
        {
            yield return error;
        }

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

    public (decimal p1, decimal p2) CalcularPorcentajes()
    {
        if (ConsumoTotal == 0) return (0m, 0m);

        decimal total = ConsumoTotal;
        var consumos = CalcularConsumos();

        decimal porcentaje1 = (consumos.c1 / total) * 100m;
        decimal porcentaje2 = (consumos.c2 / total) * 100m;

        return (porcentaje1, porcentaje2);
    }

    public decimal CalcularAseo()
    {
        return ValorAseo / 2m;
    }
    public (decimal luz1, decimal luz2, decimal aseo, decimal total1, decimal total2, decimal totalLuz) CalcularPagos()
    {
        var aseoIndiv = CalcularAseo();
        decimal valorLuz = ValorRecibo - ValorAseo;
        decimal l1 = 0m, l2 = 0m;
        decimal totalLuz = 0m;
   

        if (ConsumoTotal > 0)
        {
            var porcentajes = CalcularPorcentajes();
            l1 = Math.Round((porcentajes.p1 / 100m) * valorLuz, 0);
            l2 = Math.Round((porcentajes.p2 / 100m) * valorLuz, 0);
            totalLuz = l1 + l2;
        }
        return (l1, l2, aseoIndiv, l1 + aseoIndiv, l2 + aseoIndiv, totalLuz);
    }

    public override string ObtenerReporte()
    {
        var (c1, c2) = CalcularConsumos();
        var (p1, p2) = CalcularPorcentajes();
        var (l1, l2, aseoIndiv, total1, total2, totalLuz) = CalcularPagos();
        var aseo = CalcularAseo();

        return $@"------ Esta es la informacion de tu recibo de la luz: ------
Valor Total del Recibo: {ValorRecibo:C}
Valor Total del Aseo: {ValorAseo:C}

Lectura Anterior del Contador: {ConsumoInicial}
Lectura Actual del Contador: {ConsumoFinal}
Valor Kw del Recibo: {ValorKw:C0}
Total Aseo Por Apartamento: {aseo:C0}
Total Kw Consumidos Durante el Mes: {ConsumoTotal}

Apto 1 Consumo: {c1} kw | Porcentaje: {p1:F2}%
Apto 2 Consumo: {c2} kw | Porcentaje: {p2:F2}%

Apto 1 Pago: {total1:C0}
Apto 2 Pago: {total2:C0}";
    }
}
