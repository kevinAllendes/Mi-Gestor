using AutoMapper;
using Gestor.Models;

namespace Gestor.Servicios
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
        }
    }
}
