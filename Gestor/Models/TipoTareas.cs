namespace MiGestor
{
    public class TipoTareas
    {
        public int IdTarea {get;set;}
        public string Descripcion {get; set;}
        public int IdUsuarioCreacion {get; set;}
        public DateTime FechaCreacion {get;set;}
        public DateTime FechaFinalizacionPropuesta {get; set;}
        public DateTime FechaFinalizacion {get; set;}
        public int UsuarioAsignadoID {get;set;}
        public int IdEstadoTarea {get;set;}
        public string Detalle {get;set;}

    }
}