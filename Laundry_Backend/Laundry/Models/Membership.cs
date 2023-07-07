using System;
using System.Collections.Generic;

namespace Laundry.Models
{
    public partial class Membership
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
