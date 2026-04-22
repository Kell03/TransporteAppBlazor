using AutoMapper;
using Domain.Dto;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class RolPermisoService : BaseService<RolPermiso, RolPermisoDto>
    {
        public RolPermisoService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {


        


    }
    }
}
