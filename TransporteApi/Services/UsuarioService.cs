using AutoMapper;
using Domain.Dto;
using Microsoft.EntityFrameworkCore;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class UsuarioService : BaseService<Usuario, UsuarioDto>
    {
        public UsuarioService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }

        public override async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var entities = await _appDbContext.Usuarios.Include(x => x.Rol).ToListAsync();
            return _mapper.Map<IEnumerable<UsuarioDto>>(entities);
        }



        public  async Task<Usuario> Auth(string email)
        {

            try
            {
                Usuario entities = new Usuario();
                 entities = await _appDbContext.Usuarios.Where(x => x.Email == email).FirstOrDefaultAsync();
                if (entities != null)
                {
              

                    return entities;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
