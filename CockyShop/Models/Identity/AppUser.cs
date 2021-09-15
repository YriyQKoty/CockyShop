using System.Collections;
using System.Collections.Generic;
using CockyShop.Models.App;
using Microsoft.AspNetCore.Identity;

namespace CockyShop.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public ICollection<Order> Orders {get;set;} = new List<Order>();
    }
}