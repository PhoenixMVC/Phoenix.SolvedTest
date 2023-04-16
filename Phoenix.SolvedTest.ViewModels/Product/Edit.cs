using System.ComponentModel.DataAnnotations;

namespace Phoenix.SolvedTest.ViewModels.Product
{

    public class Edit
    {
        public int Id { get; set; }
        [Display(Name = "نام محصول")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }
    }
}
