using MIACApi.DTO;
using System;
using System.Collections.Generic;

namespace MIACApi.Models;

public partial class Seller
{
    public SellerDTO ToDTO()
    {
        return new SellerDTO
        {
            IdSeller = this.IdSeller,
            Name = this.Name,
            Surname = this.Surname,
            Patronymic = this.Patronymic,
            RegistrationDate = this.RegistrationDate,
        };
    }
}
