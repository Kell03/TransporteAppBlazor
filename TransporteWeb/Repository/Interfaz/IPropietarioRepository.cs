using Domain.Dto;

namespace TransporteWeb.Repository.Interfaz
{
    public interface IPropietarioRepository : IBaseRepository<Propietario, PropietarioDto>
    {
        Task<UploadResultDto> UploadExcelAsync(Stream fileStream, string fileName);
    }
}
