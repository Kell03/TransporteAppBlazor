using AutoMapper;
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

        public virtual async Task<T> FindAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _dbSet.ToListAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
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
