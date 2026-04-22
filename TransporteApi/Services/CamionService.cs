using AutoMapper;
using Domain.Dto;
using Microsoft.EntityFrameworkCore;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class CamionService : BaseService<Camion, CamionDto>
    {
        public CamionService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }


        public override async Task<IEnumerable<CamionDto>> GetAllAsync()
        {
            var entities = await _appDbContext.Camiones.Include(x => x.Propietario).ToListAsync();
            return _mapper.Map<IEnumerable<CamionDto>>(entities);
        }



        public virtual async Task<CamionDto> GetByIdAsync(int id)
        {
            var entity = await _appDbContext.Camiones.Include(x => x.Propietario).Where(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<CamionDto>(entity);
        }


    }
}
