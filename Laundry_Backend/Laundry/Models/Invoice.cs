using System;
using System.Collections.Generic;

namespace Laundry.Models
{
    public partial class Invoice
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Price { get; set; }
        public bool? IsPay { get; set; }
        public int? NumberOfItems { get; set; }
        public int? WeightOfItems { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Package? TypeNavigation { get; set; }
    }
}
