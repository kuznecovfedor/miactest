using MIACApi.Models;

namespace MIACApi.DTO
{
    public class SellerDTO
    {
        public int IdSeller { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? Patronymic { get; set; }
        public DateOnly RegistrationDate { get; set; }
    }
}
