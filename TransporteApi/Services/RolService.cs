using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System.Data;
using Domain.Dto;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class RolService : BaseService<Rol, RolDto>
    {
        public RolService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {





        }


        public virtual async Task<RolDto> GetByIdAsync(int id)
        {
            var entity = await _appDbContext.Roles.Where(x => x.Id == id).Include(x => x.RolesPermisos).FirstOrDefaultAsync();
            return _mapper.Map<RolDto>(entity);
        }

        public override async Task<IEnumerable<RolDto>> GetAllAsync()
        {
            var entities = await _appDbContext.Roles.Include(x => x.RolesPermisos).ToListAsync();
            return _mapper.Map<IEnumerable<RolDto>>(entities);
        }

        public virtual async Task<RolDto> UpdateAsync(Rol entity)
        {

            try
            {
                // 1. Eliminar TODOS los permisos actuales
                var permisosBD = await _appDbContext.Roles_Permiso
                    .Where(x => x.Rol_Id == entity.Id)
                    .ToListAsync();

                _appDbContext.Roles_Permiso.RemoveRange(permisosBD);

                // 2. Agregar los NUEVOS permisos (todos con ID=0)
     
                // 3. Actualizar el rol
                _appDbContext.Roles.Update(entity);
                await _appDbContext.SaveChangesAsync();

                return _mapper.Map<RolDto>(entity);
            }
            catch(Exception ex)
            {
                return default(RolDto);
            }

          
        }

    }
}
