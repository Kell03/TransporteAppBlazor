using AutoMapper;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;


using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class ConductorService : BaseService<Conductor, ConductorDto>
    {
        public ConductorService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }

        public override async Task<IEnumerable<ConductorDto>> GetAllAsync(int idempresa = 0)
        {
            var entities = await _appDbContext.Conductores.Where(x => x.EmpresaId == idempresa).Include(x => x.Propietario).Include(x => x.Camion).ToListAsync();
            return _mapper.Map<IEnumerable<ConductorDto>>(entities);
        }

        public virtual async Task<ConductorDto> GetByCedulaAsync(string cedula, int idempresa)
        {
            var entity = await _appDbContext.Conductores.FirstOrDefaultAsync(p => p.Cedula == cedula && p.EmpresaId == idempresa);
            return _mapper.Map<ConductorDto>(entity);
        }

        public virtual async Task<ConductorDto> GetByIdAsync(int id, int idempresa)
        {
            var entity = await _appDbContext.Conductores.Include(x => x.Propietario).Include(x => x.Camion).Where(x => x.Id == id && x.EmpresaId == idempresa).FirstOrDefaultAsync();
            return _mapper.Map<ConductorDto>(entity);
        }

        public virtual async Task<ConductorDto> GetByNombreAsync(string nombre, int idempresa)
        {
            var entity = await _appDbContext.Conductores.FirstOrDefaultAsync(x =>
        (x.Nombre + " " + x.Apellido).Contains(nombre) && x.EmpresaId == idempresa);

            return _mapper.Map<ConductorDto>(entity);
        }
    }
}
