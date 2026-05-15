using Domain.Dto;

namespace TransporteApi.Services
{
    public interface IServices<T, TDto>
    {
        // Métodos que usarán ambos tipos
        Task<TDto> GetByIdAsync( int id, int idempresa = 0);
        Task<T> FindAsync(int id, int idempresa = 0);
        Task<IEnumerable<TDto>> GetAllAsync(int idempresa = 0);
        Task<TDto> CreateAsync(T entity);
        Task<TDto> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
