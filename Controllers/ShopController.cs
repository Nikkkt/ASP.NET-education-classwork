using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Data.Entities;
using ASP.NET_Classwork.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            ProductGroup? group = null;
            var source = _dataContext.Groups.Include(g => g.Products).Where(g => g.DeleteDt == null);
            group = source.FirstOrDefault(g => g.Slug == id);
            if (group == null)
            {
                try
                {
                    group = source.FirstOrDefault(g => g.Id == Guid.Parse(id));
                }
                catch { }
            }
            if (group == null)
            {
                return View("Page404");
            }

            ShopGroupPageModel model = new() { ProductGroup = group };
            return View(model);
        }
    }
}
