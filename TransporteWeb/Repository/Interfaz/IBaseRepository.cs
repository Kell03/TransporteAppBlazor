using Domain.Dto;

namespace TransporteWeb.Repository.Interfaz
{
    public interface IBaseRepository<T, TDto>
    where T : class
    where TDto : class
    {


        Task<List<TDto>> GetAllAsync();
        Task<TDto> UpdateAsync(TDto entity);
        Task<TDto> GetById(int id);
        Task<bool> DeleteAsync(int id);
        Task<TDto> SaveAsync(TDto entity);

        Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName);
        Task<Stream> ExportExcelAsync(ExportRequest exportRequest);

    }
}
