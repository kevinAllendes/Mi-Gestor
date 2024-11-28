using Dapper;
using MiGestor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace MiGestor.Servicios
{
    public interface IRepositorioTipoClientes
    {
        //Definimos la referencia en la interface
        List<TipoClientes> ObtenerClientes();

    }

    public class RepositorioTipoClientes: IRepositorioTipoClientes
    {
        private readonly string connectionString;

        public RepositorioTipoClientes(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public List<TipoClientes> ObtenerClientes()
        {
            

            return new List<TipoClientes>
            {
                new TipoClientes{
                nombre = "Pedro",
                apellido = "Baños",
                email = "Pedro@gmail.com"
            },
            new TipoClientes{
                nombre = "Laura",
                apellido = "Garcia",
                email = "LGarcia@gmail.com"
            },
            new TipoClientes(){
                nombre = "Jorge",
                apellido = "Ibarra",
                email = "JIbarra@gmail.com"
            }
            

            };
        }
    }
}
