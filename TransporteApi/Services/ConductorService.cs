using AutoMapper;
using Domain.Dto;
using Microsoft.EntityFrameworkCore;


using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class ConductorService : BaseService<Conductor, ConductorDto>
    {
        public ConductorService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }

        public override async Task<IEnumerable<ConductorDto>> GetAllAsync()
        {
            var entities = await _appDbContext.Conductores.Include(x => x.Propietario).Include(x => x.Camion).ToListAsync();
            return _mapper.Map<IEnumerable<ConductorDto>>(entities);
        }

        public virtual async Task<ConductorDto> GetByCedulaAsync(string cedula)
        {
            var entity = await _appDbContext.Conductores.FirstOrDefaultAsync(p => p.Cedula == cedula);
            return _mapper.Map<ConductorDto>(entity);
        }

        public virtual async Task<ConductorDto> GetByIdAsync(int id)
        {
            var entity = await _appDbContext.Conductores.Include(x => x.Propietario).Include(x => x.Camion).Where(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<ConductorDto>(entity);
        }
    }
}
