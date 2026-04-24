using System.ComponentModel.DataAnnotations;

namespace AppFacturas.Models;

public class Agua : Factura
{
    [Required]
    [Range(0.01, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    [DataType(DataType.Currency)]
    public decimal ValorAcueducto { get; set; }

    [Required]
    [Range(0.01, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    [DataType(DataType.Currency)]
    public decimal ValorAlcantarillado { get; set; }

    [Required]
    [Range(0.01, 9999999, ErrorMessage = "El valor debe ser mayor a cero")]
    [DataType(DataType.Currency)]
    public decimal OtroCobro { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "El valor debe ser mayor a cero")]
    public int NumeroApartamentos { get; set; }

    // Esta lista se llenará automáticamente gracias al JS y el Model Binder
    public List<LecturaApto> ListaAptos { get; set; } = new List<LecturaApto>();
    public Agua() { }
    public Agua(decimal valorRecibo, int lecturaInicialRecibo, int lecturaFinalRecibo, DateTime fechaInicial, DateTime fechaFinal, decimal valorAcueducto, decimal valorAlcantarillado, decimal otros, int numAptos, List<LecturaApto> lista) : base(valorRecibo, lecturaInicialRecibo, lecturaFinalRecibo, fechaInicial, fechaFinal)
    {
        ValorAcueducto = valorAcueducto;
        ValorAlcantarillado = valorAlcantarillado;
        OtroCobro = otros;
        NumeroApartamentos = numAptos;
        ListaAptos = lista ?? new List<LecturaApto>();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var error in base.Validate(validationContext))
        {
            yield return error;
        }

        foreach (var apto in ListaAptos)
        {
            if (apto.TerceraLectura < apto.PrimeraLectura)
            {
                yield return new ValidationResult(
                    $"La tercera lectura del apartamento no puede ser menor a la primera. (Apto: {ListaAptos.IndexOf(apto) + 1})",
                    new[] { nameof(ListaAptos) }
                );
            }
        }
    }

    public decimal CalcularMetroPorConcepto(decimal valorDelServicio)
    {
        // Sumar el consumo real de todos los apartamentos usando LINQ
        int totalConsumo = ListaAptos.Sum(a => a.CosumoReal);
        int consumoRecibo = LecturaFinalRecibo - LecturaInicialRecibo;

        /*if (totalConsumo == 0) return 0;
        return valorDelServicio / totalConsumo;*/

        return valorDelServicio / consumoRecibo; 
    }

    public override string ObtenerReporte()
    {
        // Valores unitarios
        decimal vUnitAcueducto = CalcularMetroPorConcepto(ValorAcueducto);
        decimal vUnitAlcantarillado = CalcularMetroPorConcepto(ValorAlcantarillado);

        // Acumulador para el detalle de los apartamentos
        string detalleApartamentos = "";

        for (int i = 0; i < ListaAptos.Count; i++)
        {
            var apto = ListaAptos[i];
            decimal subAcue = apto.CosumoReal * vUnitAcueducto;
            decimal subAlcan = apto.CosumoReal * vUnitAlcantarillado;
            decimal totalApto = subAcue + subAlcan;

            detalleApartamentos += $@"
APTO {i + 1}:
   Consumo:       {apto.CosumoReal} m3
   Acueducto:     {subAcue:C0}
   Alcantarillado: {subAlcan:C0}
   VALOR A PAGAR: {totalApto:C0}
--------------------------------------------------";
        }

        return $@"
==================================================
       REPORTE DE CONSUMO DE AGUA 
==================================================
[ DATOS DEL RECIBO ]
--------------------------------------------------
Periodo:        {FechaInicial:d} al {FechaFinal:d}
Valor Total:    {ValorRecibo:C0}
Conceptos:      Acue: {ValorAcueducto:C0} | Alca: {ValorAlcantarillado:C0}

Lectura Recibo: {LecturaInicialRecibo} m3 - {LecturaFinalRecibo} m3
Total Metros:   {LecturaFinalRecibo - LecturaInicialRecibo} m3
--------------------------------------------------

[ DISTRIBUCIÓN POR APARTAMENTO ]
--------------------------------------------------{detalleApartamentos}

VALOR PROPIETARIO (Otros): {OtroCobro:C0}
==================================================";
    }

}

// Clase de apoyo para los apartamentos
public class LecturaApto
{
    public int PrimeraLectura { get; set; }
    public int SegundaLectura { get; set; }
    public int TerceraLectura { get; set; }
    public int CosumoReal => TerceraLectura - PrimeraLectura;

    public LecturaApto() { }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TerceraLectura < PrimeraLectura)
        {
            yield return new ValidationResult(
                "La tercera lectura del contador no puede ser menor a la primera.",
                new[] { nameof(TerceraLectura) }
                );
        }

    }
}
