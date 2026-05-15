using AutoMapper;
using Domain.Dto;
using Microsoft.EntityFrameworkCore;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class BaseService<T, TDto> : IServices<T, TDto> where T : class
    {
        protected readonly AppDbContext _appDbContext;
        protected readonly IMapper _mapper;
        protected readonly DbSet<T> _dbSet;

        public BaseService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _dbSet = _appDbContext.Set<T>();
        }

        private bool TienePropiedadIdEmpresa()
        {
            return typeof(T).GetProperty("EmpresaId") != null;
        }


        public virtual async Task<TDto> CreateAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _appDbContext.SaveChangesAsync();
                return _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public virtual async Task<T> FindAsync(int id, int idempresa = 0)
        {
            IQueryable<T> query = _dbSet;

            // Verificar si la entidad tiene la propiedad IdEmpresa

            if (TienePropiedadIdEmpresa() && idempresa > 0)
            {
                query = query.Where(e =>
                    EF.Property<int>(e, "Id") == id &&
                    EF.Property<int>(e, "EmpresaId") == idempresa);
            }
            else
            {
                query = query.Where(e => EF.Property<int>(e, "Id") == id);
            }

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(int idempresa = 0)
        {
            IQueryable<T> query = _dbSet;

            // Si se pasa un idempresa válido, filtrar
            if (idempresa > 0 && TienePropiedadIdEmpresa())
            {
                query = query.Where(e => EF.Property<int>(e, "EmpresaId") == idempresa);
            }

            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<TDto> GetByIdAsync(int id, int idempresa = 0)
        {
            IQueryable<T> query = _dbSet;

            // Si se pasa un idempresa válido, filtrar por ambos
            if (idempresa > 0 && TienePropiedadIdEmpresa())
            {
                query = query.Where(e =>
                    EF.Property<int>(e, "Id") == id &&
                    EF.Property<int>(e, "EmpresaId") == idempresa);
            }
            else
            {
                query = query.Where(e => EF.Property<int>(e, "Id") == id);
            }

            var entity = await query.FirstOrDefaultAsync();
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _appDbContext.SaveChangesAsync();
                return _mapper.Map<TDto>(entity);
            }
            catch (Exception ex)
            {
                return default(TDto);
            }
        }


    }
}
