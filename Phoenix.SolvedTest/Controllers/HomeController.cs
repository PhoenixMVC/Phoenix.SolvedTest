
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phoenix.Data.Repository;

namespace Phoenix.SolvedTest.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IDbContext _context;
        public HomeController(IDbContext context)
        {
            _context = context;
            
        }
      
        public IActionResult Index()
        {

             return View();
        }

   
      
    }
}
