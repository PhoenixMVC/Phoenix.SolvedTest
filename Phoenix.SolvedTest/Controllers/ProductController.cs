using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phoenix.Data.Repository;
using Phoenix.SolvedTest.Models;
using Phoenix.SolvedTest.ViewModels.Product;
using System;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Phoenix.SolvedTest.Controllers
{
    [AllowAnonymous]
    public class ProductController : Controller
    {

        private readonly IDbContext _context;
        public ProductController(IDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index(int? page, string sort, string searchString)
        {
            var pageIndex = (page ?? 1) - 1;
            var recordsPerPage = 20;
            ViewData["pageIndex"] = pageIndex;

            var products = _context.DbSet<Product>()
                                   .Select(x => new GetProduct()
                                   {
                                       Id = x.Id,
                                       Name = x.Name,
                                       Description = x.Description
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
            products = products
                         .Skip(pageIndex * recordsPerPage)
                         .Take(recordsPerPage);

            var model = new StaticPagedList<GetProduct>(products, pageIndex + 1, recordsPerPage, totalCount);
            return View(model);
        }


        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create create)
        {
            if (ModelState.IsValid)
            {

                Product product = new Product()
                {
                    Name = create.Name,
                    Description = create.Description,
                };

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            var result = await _context.DbSet<Product>()
                                 .Where(c => c.Id == id)
                                 .Select(x => new Edit()
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Description = x.Description,

                                 })
                                 .FirstOrDefaultAsync(HttpContext.RequestAborted);

            return View(result);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Edit edit)
        {
            if (edit == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var model = new Product
                {
                    Id = edit.Id,
                    Name = edit.Name,
                    Description = edit.Description,
                };
                _context.Update(model);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index", "Product");
        }


    }
}