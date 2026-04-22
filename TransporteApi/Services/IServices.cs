namespace TransporteApi.Services
{
    public interface IServices<T, TDto>
    {
        // Métodos que usarán ambos tipos
        Task<TDto> GetByIdAsync(int id);
        Task<T> FindAsync(int id);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> CreateAsync(T entity);
        Task<TDto> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
