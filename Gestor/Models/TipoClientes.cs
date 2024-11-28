using System.ComponentModel.DataAnnotations;
using Microsoft.JSInterop.Infrastructure;

namespace MiGestor.Models;

    public class TipoClientes
    {
        //Definimos los atributos que referenciaran a cada cliente
        public int idCliente {get; set;}
        public string nombre {get; set;}
        public string apellido {get;set;}
        public string email {get; set;}
        public long dni {get; set;}
        public long cuil_cuit {get;set;}
        public long cbu {get;set;}
        public string AfipUser {get; set;}
        public string passAfip {get; set;}

    }
    
