using System;
using System.ComponentModel.DataAnnotations;

namespace Phoenix.SolvedTest.ViewModels.Report
{
    public class GetReport
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public DateTime CreateDate { get; set; }


    }
}
