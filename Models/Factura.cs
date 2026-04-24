using System.ComponentModel.DataAnnotations;

namespace AppFacturas.Models;

public abstract class Factura : IValidatableObject
{
    [Required]
    [Range(0.01, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    [DataType(DataType.Currency)]
    public decimal ValorRecibo { get; set; }

    [Required]
    [Range(0, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    public int LecturaInicialRecibo { get; set; }

    [Required]
    [Range(0, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    public int LecturaFinalRecibo { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaInicial { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaFinal { get; set; }

    public Factura()
    {
    }

    public Factura(decimal valorRecibo, int lecturaInicialRecibo, int lecturaFinalRecibo, DateTime fechaInicial, DateTime fechaFinal)
    {
        ValorRecibo = valorRecibo;
        LecturaInicialRecibo = lecturaInicialRecibo;
        LecturaFinalRecibo = lecturaFinalRecibo;
        FechaInicial = fechaInicial;
        FechaFinal = fechaFinal;
    }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaFinal < FechaInicial)
        {
            yield return new ValidationResult(
                "La fecha final no puede ser anterior a la fecha inicial.", new[] { nameof(FechaFinal) }
                );
        }

        if (LecturaFinalRecibo < LecturaInicialRecibo)
        {
            yield return new ValidationResult(
                "La lectura final del recibo no puede ser menor a la lectura inicial.", new[] { nameof(LecturaFinalRecibo) }
                );
        }
    }

    public virtual string ObtenerReporte()
    {
        return $"Factura General - Valor: {ValorRecibo:C0}";
    }

    public virtual void ShowInfo()
    {
        Console.WriteLine(ObtenerReporte());
    }
}