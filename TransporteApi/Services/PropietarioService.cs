using AutoMapper;
using Domain.Dto;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class PropietarioService : BaseService<Propietario, PropietarioDto>
    {
        public PropietarioService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }
    }
}
