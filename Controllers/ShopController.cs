using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Models.Shop;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Classwork.Controllers
{
    public class ShopController(DataContext dataContext) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        public IActionResult Index()
        {
            ShopPageModel model = new()
            {
                ProductGroups = _dataContext.Groups.Where(g => g.DeleteDt == null)
            };
            return View(model);
        }

        public IActionResult Group(String id)
        {

            ViewData["id"] = id;
            return View();
        }
    }
}
