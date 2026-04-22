using AutoMapper;
using Domain.Dto;
using TransporteApi.Models;

namespace TransporteApi.Mappers
{
    public class AutoMapper : Profile
    {

        public AutoMapper()
        {

            CreateMap<RolPermiso, RolPermisoDto>()
                .ForMember(dest => dest.Rol, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CentroDistribucion, CentroDistribucionDto>()
               .ReverseMap();


            CreateMap<Propietario, PropietarioDto>()
                .ForMember(dest => dest.Camion, opt => opt.MapFrom(src => src.Camion));
            CreateMap<PropietarioDto, Propietario>()
            .ForMember(dest => dest.Created_at, opt => opt.Ignore())
            .ForMember(dest => dest.Updated_at, opt => opt.Ignore());



            CreateMap<Rol, RolDto>()
            .ForMember(dest => dest.RolesPermisos, opt => opt.MapFrom(src => src.RolesPermisos));


            CreateMap<RolDto, Rol>()
            .ForMember(dest => dest.Created_at, opt => opt.Ignore())
            .ForMember(dest => dest.Updated_at, opt => opt.Ignore());
            //.ForMember(dest => dest.RolesPermisos, opt => opt.Ignore());

            CreateMap<UsuarioDto, Usuario>()
              .ForMember(dest => dest.Rol, opt => opt.Ignore());
            CreateMap<Usuario, UsuarioDto>();


            CreateMap<CamionDto, Camion>()
              .ForMember(dest => dest.Propietario, opt => opt.Ignore()) 
              .ForPath(dest => dest.Propietario.Camion, opt => opt.Ignore()); 
            CreateMap<Camion, CamionDto>();

            CreateMap<ConductorDto, Conductor>()
              .ForMember(dest => dest.Propietario, opt => opt.Ignore())
              .ForPath(dest => dest.Propietario.Camion, opt => opt.Ignore());
            CreateMap<Conductor, ConductorDto>();



            CreateMap<GuiaDto, Guia>()
            .ForMember(dest => dest.Conductor, opt => opt.Ignore())
            .ForMember(dest => dest.Camion, opt => opt.Ignore())
            .ForMember(dest => dest.Origen, opt => opt.Ignore())
            .ForMember(dest => dest.Destino, opt => opt.Ignore());
            CreateMap<Guia, GuiaDto>();



        }
    }
}
