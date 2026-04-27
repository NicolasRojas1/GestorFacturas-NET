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
    public int ConsumoApto1 { get; set; }
    public int ConsumoApto2 { get; set; }
    public decimal PorcentajeApto1 { get; set; }
    public decimal PorcentajeApto2 { get; set; }
    public decimal AseoPorApto => ValorAseo / 2m;
    public decimal SubTotalLuz => ValorRecibo - ValorAseo;
    public decimal ValorLuzApto1 { get; set; }
    public decimal ValorLuzApto2 { get; set; }
    public decimal TotalApto1 { get; set; }
    public decimal TotalApto2 { get; set; }

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
    protected void CalcularPagos() // Cambiado a protected
    {
        if (ConsumoTotal > 0)
        {
            this.ValorLuzApto1 = Math.Round(this.PorcentajeApto1 / 100m * this.SubTotalLuz, 0);
            // El segundo es la diferencia para que no se pierda ni un peso por redondeo
            this.ValorLuzApto2 = this.SubTotalLuz - this.ValorLuzApto1;

            this.TotalApto1 = this.ValorLuzApto1 + this.AseoPorApto;
            this.TotalApto2 = this.ValorLuzApto2 + this.AseoPorApto;
            return;
        }
        // Si no hay consumo, cada uno paga su mitad de aseo
        this.ValorLuzApto1 = 0;
        this.ValorLuzApto2 = 0;
        this.TotalApto1 = this.AseoPorApto;
        this.TotalApto2 = this.AseoPorApto;
    }
    public void ProcesarFactura()
    {
        CalcularConsumos();
        CalcularPorcentajes();
        CalcularPagos();
    }

    protected override string ObtenerReporte()
    {
        return $@"------ Esta es la informacion de tu recibo de la luz: ------
Valor Total del Recibo: {ValorRecibo:C}
Valor Total del Aseo: {ValorAseo:C}

Lectura Anterior del Contador: {ConsumoInicial}
Lectura Actual del Contador: {ConsumoFinal}
Valor Kw del Recibo: {ValorKw:C0}
Total Aseo Por Apartamento: {AseoPorApto:C0}
Total Kw Consumidos Durante el Mes: {ConsumoTotal}

Apto 1 Consumo: {ConsumoApto1} kw | Porcentaje: {PorcentajeApto1:F2}%
Apto 2 Consumo: {ConsumoApto2} kw | Porcentaje: {PorcentajeApto2:F2}%

Apto 1 Pago: {TotalApto1:C0}
Apto 2 Pago: {TotalApto2:C0}";
    }
}
