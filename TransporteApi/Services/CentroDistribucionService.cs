using AutoMapper;
using Domain.Dto;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class CentroDistribucionService : BaseService<CentroDistribucion, CentroDistribucionDto>
    {
        public CentroDistribucionService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }
    }
}
