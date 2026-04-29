namespace Domain.Entities
{
    public class Viaje
    {

        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Moneda { get; set; }

        public Viaje(string nombre, string descripcion, DateTime fechaCreacion, string moneda)
        {

            Nombre = nombre;
            Descripcion = descripcion;
            FechaCreacion = fechaCreacion;
            Moneda = moneda;
        }

    }

}