using System;
using System.Collections.Generic;

namespace Laundry.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Invoices = new HashSet<Invoice>();
            Memberships = new HashSet<Membership>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Mail { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsMember { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }
    }
}
