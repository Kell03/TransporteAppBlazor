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

        public virtual async Task<CentroDistribucionDto> GetByCodigoAsync(string codigo)
        {
            var entity = await _appDbContext.Centro_distribucion.FirstOrDefaultAsync(p => p.Codigo == codigo);
            return _mapper.Map<CentroDistribucionDto>(entity);
        }
    }
}
