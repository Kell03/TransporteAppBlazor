using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Domain.Dto;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class GuiaService : BaseService<Guia, GuiaDto>
    {
        public GuiaService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }

        public override async Task<IEnumerable<GuiaDto>> GetAllAsync()
        {
            var entities = await _appDbContext.Guias
                .Include(x => x.Conductor)
                .Include(x => x.Camion)
                .Include(x => x.Origen)
                .Include(x => x.Destino)
                .ToListAsync();
            return _mapper.Map<IEnumerable<GuiaDto>>(entities);
        }
    }
}
