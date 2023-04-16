using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Phoenix.Data.Repository;
using Phoenix.SolvedTest.Models;
using Phoenix.SolvedTest.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Phoenix.SolvedTest.Controllers
{
    [AllowAnonymous]
    public class ReportController : Controller
    {

        private readonly IDbContext _context;
        public ReportController(IDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> RemovedProduct (int? page)
        {
            var today = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);

            var pageIndex = (page ?? 1) - 1;
            var recordsPerPage = 20;
            ViewData["pageIndex"] = pageIndex;

            var products = _context.DbSet<Price>()
                                   .Include(p => p.Product)
                                   .Where(p => p.CreateDate.Date == today.Date || p.CreateDate.Date == yesterday.Date)
                                   .Select(x => new GetReport()
                                   {
                                       ProductId = x.Product.Id,
                                       Name = x.Product.Name,
                                       Price = x.SalesPrice,
                                       CreateDate = x.CreateDate
                                   });

            List<GetReport> removedProduct = new List<GetReport>();
            foreach (var item in products.ToList().GroupBy(p=>p.ProductId))
            {
                if (item.Count() == 1)
                {
                   if( item.First().CreateDate.Date == yesterday.Date)
                    {
                        removedProduct.Add(item.First());
                    }
                }
            }
            int totalCount = removedProduct.Count();
            var modelPaging = removedProduct
                          .Skip(pageIndex * recordsPerPage)
                          .Take(recordsPerPage);

            var model = new StaticPagedList<GetReport>(modelPaging, pageIndex + 1, recordsPerPage, totalCount);

            return View(model);
        }

        public async Task<IActionResult> NewProduct(int? page)
        {
            var today = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);

            var pageIndex = (page ?? 1) - 1;
            var recordsPerPage = 20;
            ViewData["pageIndex"] = pageIndex;

            var products = _context.DbSet<Price>()
                                   .Include(p => p.Product)
                                   .Where(p => p.CreateDate.Date == today.Date || p.CreateDate.Date == yesterday.Date)
                                   .Select(x => new GetReport()
                                   {
                                       ProductId = x.Product.Id,
                                       Name = x.Product.Name,
                                       Price = x.SalesPrice,
                                       CreateDate = x.CreateDate
                                   });

            List<GetReport> newProduct = new List<GetReport>();
            foreach (var item in products.ToList().GroupBy(p => p.ProductId))
            {
                if (item.Count() == 1)
                {
                    if (item.First().CreateDate.Date == today.Date)
                    {
                        newProduct.Add(item.First());
                    }
                }
            }
            int totalCount = newProduct.Count();
            var modelPaging = newProduct
                          .Skip(pageIndex * recordsPerPage)
                          .Take(recordsPerPage);

            var model = new StaticPagedList<GetReport>(modelPaging, pageIndex + 1, recordsPerPage, totalCount);

            return View(model);
        }

        public async Task<IActionResult> ChangePriceProduct(int? page)
        {
            var today = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);

            var pageIndex = (page ?? 1) - 1;
            var recordsPerPage = 20;
            ViewData["pageIndex"] = pageIndex;

            var products = _context.DbSet<Price>()
                                   .Include(p => p.Product)
                                   .Where(p => p.CreateDate.Date == today.Date || p.CreateDate.Date == yesterday.Date)
                                   .Select(x => new GetReport()
                                   {
                                       ProductId = x.Product.Id,
                                       Name = x.Product.Name,
                                       Price = x.SalesPrice,
                                       CreateDate = x.CreateDate
                                   });

            List<GetReport> changePriceProduct = new List<GetReport>();
            foreach (var item in products.ToList().GroupBy(p => p.ProductId))
            {
                if (item.Count() == 2)
                {
                    if (item.Last().Price != item.First().Price)
                    {
                        changePriceProduct.Add(item.First());
                    }
                }
            }

            int totalCount = changePriceProduct.Count();

            var modelPaging = changePriceProduct.Skip(pageIndex * recordsPerPage).Take(recordsPerPage);

            var model = new StaticPagedList<GetReport>(modelPaging, pageIndex + 1, recordsPerPage, totalCount);

            return View(model);
        }

     

    }
}