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
            CreateMap<Seller, SellerDTO>();
        }
    }
}
