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

        // Key del objeto en S3 (no una URL): el bucket es privado, así que la URL real se
        // firma con vencimiento recién al armar el ViajeDto, nunca se persiste.
        public string? PortadaKey { get; set; }

        public ICollection<ParticipanteViaje> Participantes { get; set; } = new List<ParticipanteViaje>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
        public Viaje(string nombre, string descripcion, string moneda)
        {

            Nombre = nombre;
            Descripcion = descripcion;
            FechaCreacion = DateTime.UtcNow;
            Moneda = moneda;
        }

        public Viaje() { }
    }

}