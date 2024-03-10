using AutoMapper;
using MIACApi.DTO;
using MIACApi.Models;

namespace MIACApi.Mappers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Material, MaterialDTO>();
            CreateMap<MaterialDTO, Material>();
            CreateMap<Seller, SellerDTO>();
            CreateMap<SellerDTO, Seller>();
            CreateMap<Seller, ModifySellerDTO>();
            CreateMap<ModifySellerDTO, Seller>();
        }
    }
}
