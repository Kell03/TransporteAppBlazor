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

        public override async Task<IEnumerable<CamionDto>> GetAllAsync(int idempresa = 0, int clienteId = 0)
        {
            try
            {

                var entities = await _appDbContext.Camiones
                    .Where(x => x.EmpresaId == idempresa)
                    .Include(x => x.Propietario)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<CamionDto>>(entities);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public virtual async Task<CamionDto> GetByIdAsync(int id, int idempresa = 0)
        {
            var entity = await _appDbContext.Camiones.Include(x => x.Propietario).Where(x => x.Id == id && x.EmpresaId == idempresa).FirstOrDefaultAsync();
            return _mapper.Map<CamionDto>(entity);
        }

        public virtual async Task<CamionDto> GetByPlacaAsync(string placa, int idempresa = 0)
        {
            var entity = await _appDbContext.Camiones.FirstOrDefaultAsync(p => p.Placa1 == placa && p.EmpresaId == idempresa);
            return _mapper.Map<CamionDto>(entity);
        }


    }
}
