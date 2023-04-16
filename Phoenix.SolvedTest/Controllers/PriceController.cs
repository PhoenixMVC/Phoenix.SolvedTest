using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phoenix.Data.Repository;
using Phoenix.SolvedTest.Models;
using Phoenix.SolvedTest.ViewModels.Price;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using ExtensionMethods;
using System.Globalization;

namespace Phoenix.SolvedTest.Controllers
{
    [AllowAnonymous]
    public class PriceController : Controller
    {

        private readonly IDbContext _context;
        public PriceController(IDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index(int? page, string sort, string searchString)
        {
            var pageIndex = (page ?? 1) - 1;
            var recordsPerPage = 20;
            ViewData["pageIndex"] = pageIndex;

            var products = _context.DbSet<Product>()
                                   .Include(p => p.Prices)
                                   .Select(x => new GetProductWithPrice()
                                   {
                                       Id = x.Id,
                                       Name = x.Name,
                                       Description = x.Description,
                                       Price = (x.Prices.Count() == 0) ? "0" : x.Prices.FirstOrDefault(p => p.IsDeleted == false).SalesPrice.ToString(),
                                       CreateDate = (x.Prices.Count() == 0) ? "---" : x.Prices.FirstOrDefault(p => p.IsDeleted == false).CreateDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR"))
                                   });

            ViewData["searchString"] = searchString;
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name == searchString);
            }

            ViewData["IdSortParm"] = string.IsNullOrEmpty(sort) ? "Id_Desc" : "";
            ViewData["NameSortParm"] = sort == "Name" ? "Name_Desc" : "Name";

            switch (sort)
            {
                case "Id_Desc":
                    products = products.OrderByDescending(x => x.Id);
                    break;
                case "Name":
                    products = products.OrderBy(x => x.Name);
                    break;
                case "Name_Desc":
                    products = products.OrderByDescending(x => x.Name);
                    break;

                default:
                    products = products.OrderBy(x => x.Id);
                    break;
            }

           int totalCount = products.Count();
            var modelPaging = products
                          .Skip(pageIndex * recordsPerPage)
                          .Take(recordsPerPage);

            var model = new StaticPagedList<GetProductWithPrice>(modelPaging, pageIndex + 1, recordsPerPage, totalCount);
            return View(model);
        }


        public async Task<IActionResult> UpdatePrice(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePrice(UpdatePrice model)
        {
            model.SalesPrice = model.SalesPrice.Replace(",", "").PersianToEnglish();

            if (ModelState.IsValid)
            {

                var lastPrice = await _context.DbSet<Price>()
                                               .Include(p => p.Product)
                                               .Where(p => p.ProductId == model.ProductId &&
                                                           p.IsDeleted == false)
                                               .FirstOrDefaultAsync();

                if (lastPrice != null)
                {
                    lastPrice.DeleteDate = DateTime.Now;
                    lastPrice.IsDeleted = true;
                    _context.Update(lastPrice);
                    _context.SaveChanges();
                }


                Price price = new Price()
                {
                    CreateDate = DateTime.Now,
                    ProductId = model.ProductId,
                    SalesPrice = Convert.ToDouble(model.SalesPrice),
                    IsDeleted = false,
                };
                _context.Add(price);
                await _context.SaveChangesAsync(HttpContext.RequestAborted);

                return RedirectToAction(nameof(Index));

            }

            else
            {
                TempData["Message"] = "خطا";
                TempData["Style"] = "alert alert-danger";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}