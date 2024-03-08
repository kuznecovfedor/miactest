using MIACApi.Models;

namespace MIACApi.DTO
{
    public class MaterialDTO
    {
        public int IdMaterial { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int IdSeller { get; set; }
    }
}
