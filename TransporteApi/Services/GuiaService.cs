using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Domain.Dto;
using TransporteApi.Models;

namespace TransporteApi.Services
{
    public class GuiaService : BaseService<Guia, GuiaDto>
    {
        public GuiaService(AppDbContext appDbContext, IMapper mapper) : base(appDbContext, mapper)
        {
        }

        public override async Task<IEnumerable<GuiaDto>> GetAllAsync()
        {
            var entities = await _appDbContext.Guias
                .Include(x => x.Conductor)
                .Include(x => x.Camion)
                   .ThenInclude(c => c.Propietario)
                .Include(x => x.Origen)
                .Include(x => x.Destino)
                .ToListAsync();
            return _mapper.Map<IEnumerable<GuiaDto>>(entities);
        }

        public virtual async Task<GuiaDto> GetByIdAsync(int id)
        {
            var entity = await _appDbContext.Guias.Include(x => x.Conductor).Include(x => x.Camion).Include(x => x.Origen).Include(x => x.Destino).Where(x => x.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<GuiaDto>(entity);
        }


        public virtual async Task<GuiaDto> GetByNumeroAsync(string numero)
        {
            var entity = await _appDbContext.Guias.Include(x => x.Conductor).Include(x => x.Camion).Include(x => x.Origen).Include(x => x.Destino).Where(x => x.Numero_guia == numero).FirstOrDefaultAsync();
            return _mapper.Map<GuiaDto>(entity);
        }

        public IQueryable<GuiaDto> AplicarFiltro(IQueryable<GuiaDto> query, FilterDefinitionDto filtro)
        {
            var propertyName = filtro.PropertyName;
            var operatorType = filtro.Operator;
            var value = filtro.Value?.ToString();

            if (string.IsNullOrWhiteSpace(value))
                return query;

            switch (propertyName)
            {
                case "Numero_guia":
                    return query.Where(x => x.Numero_guia.Contains(value));

                case "Tipo":
                    return query.Where(x => x.Tipo.Contains(value));

                case "Status":
                    return query.Where(x => x.Status.Contains(value));

                case "Origen.Nombre":
                    return query.Where(x => x.Origen.Nombre.Contains(value));

                case "Destino.Nombre":
                    return query.Where(x => x.Destino.Nombre.Contains(value));

                case "Conductor.NombreCompleto":
                    return query.Where(x => x.Conductor.NombreCompleto.Contains(value));

                case "Descripcion":
                    return query.Where(x => x.Descripcion != null && x.Descripcion.Contains(value));


                case nameof(GuiaDto.Fecha):
                    if (DateTime.TryParse(value, out var fecha))
                    {

                        if (operatorType == "is")
                            return query.Where(x => x.Fecha.Date == fecha.Date);

                        if (operatorType == "is not")
                            return query.Where(x => x.Fecha.Date != fecha.Date);

                        if (operatorType == "is after")
                            return query.Where(x => x.Fecha.Date > fecha.Date);

                        if (operatorType == "is on or after")
                            return query.Where(x => x.Fecha.Date >= fecha.Date);

                        if (operatorType == "is before")
                            return query.Where(x => x.Fecha.Date < fecha.Date);

                        if (operatorType == "is on or before")
                            return query.Where(x => x.Fecha.Date <= fecha.Date);

                        if (operatorType == "is empty")
                            return query.Where(x => x.Fecha == null);

                        if (operatorType == "is not empty")
                            return query.Where(x => x.Fecha != null);
                    }
                    return query; // Si no se pudo parsear la fecha

                default:
                    return query;
            }
        }
    }
}
