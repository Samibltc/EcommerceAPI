using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Entities.Base;

namespace EcommerceAPI.Entities
{
    public class Cart : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
