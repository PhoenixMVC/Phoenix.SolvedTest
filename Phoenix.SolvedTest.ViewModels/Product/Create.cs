using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Phoenix.SolvedTest.ViewModels.Product
{
    public class Create
    {

        [Display(Name = "نام محصول")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

    }
}
