using System.Collections.Generic;
using System;
namespace Domain.Entities

{
    public class Viaje
    {

        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Moneda { get; set; }

        // Rango opcional del viaje y su estado ("Activo" | "Finalizado").
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = "Activo";

        // Key del objeto en S3 (no una URL): el bucket es privado, así que la URL real se
        // firma con vencimiento recién al armar el ViajeDto, nunca se persiste.
        public string? PortadaKey { get; set; }

        public ICollection<ParticipanteViaje> Participantes { get; set; } = new List<ParticipanteViaje>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
        public Viaje(string nombre, string descripcion, string moneda, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {

            Nombre = nombre;
            Descripcion = descripcion;
            FechaCreacion = DateTime.UtcNow;
            Moneda = moneda;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Estado = "Activo";
        }

        public Viaje() { }
    }

}