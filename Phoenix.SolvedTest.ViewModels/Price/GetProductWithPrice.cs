using System.ComponentModel.DataAnnotations;

namespace Phoenix.SolvedTest.ViewModels.Price
{
    public class GetProductWithPrice
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Price { get; set; }
        public string CreateDate { get; set; }


    }
}
