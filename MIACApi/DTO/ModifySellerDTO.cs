namespace MIACApi.DTO
{
    public class ModifySellerDTO
    {
        public int IdSeller { get; set; }
        public string Login { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? Patronymic { get; set; }
    }
}