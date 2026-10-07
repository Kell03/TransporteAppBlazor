using AutoMapper;
using Domain.Dto;
using Microsoft.EntityFrameworkCore;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class CentroDistribucionService : BaseService<CentroDistribucion, CentroDistribucionDto>
    {
        public CentroDistribucionService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }


        public override async Task<IEnumerable<CentroDistribucionDto>> GetAllAsync(int idempresa = 0)
        {
            var entities = await _appDbContext.Centro_distribucion.Where(x => x.EmpresaId == idempresa).Include(x => x.Cliente).ToListAsync();
            return _mapper.Map<IEnumerable<CentroDistribucionDto>>(entities);
        }
        public virtual async Task<CentroDistribucionDto> GetByCodigoAsync(string codigo)
        {
            var entity = await _appDbContext.Centro_distribucion.FirstOrDefaultAsync(p => p.Codigo == codigo);
            return _mapper.Map<CentroDistribucionDto>(entity);
        }


        public virtual async Task<CentroDistribucionDto> GetByIdAsync(int id, int idempresa)
        {
            var entity = await _appDbContext.Centro_distribucion.Include(x => x.Cliente).Where(x => x.Id == id && x.EmpresaId == idempresa).FirstOrDefaultAsync();
            return _mapper.Map<CentroDistribucionDto>(entity);
        }
    }
}
