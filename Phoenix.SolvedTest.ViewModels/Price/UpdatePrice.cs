using System.ComponentModel.DataAnnotations;

namespace Phoenix.SolvedTest.ViewModels.Price
{

    public class UpdatePrice
    {
        public int ProductId { get; set; }
  
        [Display(Name = "قیمت محصول")]
        public string SalesPrice { get; set; }

    }
}
