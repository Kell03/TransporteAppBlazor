using AutoMapper;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class ClienteService : BaseService<Cliente, ClienteDto>
    {
        public ClienteService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }



        public override async Task<IEnumerable<ClienteDto>> GetAllAsync(int idempresa = 0)
        {
            try
            {

                var entities = await _appDbContext.Clientes
                    .Where(x => x.EmpresaId == idempresa)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<ClienteDto>>(entities);
            }
            catch(Exception ex)
            {
                throw;
            }   
        }

        public virtual async Task<ClienteDto> GetByIdAsync(int id, int idempresa)
        {
            var entity = await _appDbContext.Clientes.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<ClienteDto>(entity);
        }



    }
}
