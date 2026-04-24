# 📊 Gestor de Facturas - .NET MVC

Este proyecto es una aplicación web desarrollada en **ASP.NET Core MVC** diseñada para gestionar y liquidar consumos de servicios públicos (Gas, Luz, Agua) en un complejo de apartamentos. La aplicación se enfoca en la precisión de los cálculos y la integridad de los datos mediante validaciones avanzadas.

---

## 🛠️ Tecnologías y Conceptos Aplicados

* **Framework:** .NET 8 / ASP.NET Core MVC.
* **Lenguaje:** C#.
* **Arquitectura:** Modelo-Vista-Controlador (MVC).
* **POO:** Uso extensivo de Herencia, Polimorfismo y Clases Abstractas.
* **Validaciones:** * **Data Annotations:** Para integridad básica del formulario.
    * **IValidatableObject:** Implementación de lógica de negocio compleja (validaciones cruzadas entre campos).

---

## 🚀 Características Principales

* **Jerarquía de Clases:** Una clase base `Factura` que centraliza propiedades comunes y clases derivadas para servicios específicos (ej. `Gas`) con sus propias reglas de cálculo.
* **Validación de Negocio:** Sistema que impide el procesamiento de datos incoherentes (ej. fechas finales menores a las iniciales o lecturas de contador imposibles).
* **Interfaz Dinámica:** Uso de **Tag Helpers** para una vinculación de datos segura y retroalimentación inmediata al usuario en caso de errores.

---

## 📖 Aprendizajes Clave

Este proyecto me permitió profundizar en el funcionamiento interno del **Pipeline de Validación de ASP.NET Core**, entendiendo el comportamiento de `ModelState`, el uso eficiente de la memoria mediante `IEnumerable` con `yield return`, y la importancia del tipado fuerte en las vistas Razor.

---

## 💻 Instalación y Ejecución

1. Clonar el repositorio:
   ```bash
   git clone [https://github.com/NicolasRojas1/GestorFacturas-NET.git]
