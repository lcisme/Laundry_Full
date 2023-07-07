using System;
using System.Collections.Generic;

namespace Laundry.Models
{
    public partial class Package
    {
        public Package()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
