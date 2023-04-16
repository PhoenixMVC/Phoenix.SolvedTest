using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Phoenix.SolvedTest.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public DateTime FetchDate{get; set;}
        public virtual ICollection<Price> Prices { get; set; } = new HashSet<Price>();

    }
}