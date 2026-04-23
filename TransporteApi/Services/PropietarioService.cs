using AutoMapper;
using Domain.Dto;
using Microsoft.EntityFrameworkCore;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class PropietarioService : BaseService<Propietario, PropietarioDto>
    {
        public PropietarioService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }

        public virtual async Task<PropietarioDto> GetByCodigoAsync(string codigo)
        {
            var entity = await _appDbContext.Propietario.FirstOrDefaultAsync(p => p.Codigo == codigo);
            return _mapper.Map<PropietarioDto>(entity);
        }
    }
}
