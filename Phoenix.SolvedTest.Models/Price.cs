using System;
namespace Phoenix.SolvedTest.Models
{
    public class Price
    {
        public long Id { get; set; }

        public double SalesPrice { get; set; }
               
        public int ProductId { get; set; }
        public virtual Product Product  { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeleteDate { get; set; }
        public DateTime CreateDate { get; set; }

    }
}