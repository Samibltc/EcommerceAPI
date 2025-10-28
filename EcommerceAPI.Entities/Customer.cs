using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Entities.Base;
using EcommerceAPI.Entities.ValueObjects;

namespace EcommerceAPI.Entities
{
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName  { get; set; } = null!;
        public string Email     { get; set; } = null!;
        public string? Phone    { get; set; }

        // Single address (owned type)
        public Address? Address { get; set; }

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
