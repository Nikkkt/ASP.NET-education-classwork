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
                ProductGroups = _dataContext.Groups.Include(g => g.Products).Where(g => g.DeleteDt == null)
            };
            return View(model);
        }

        public IActionResult Group(String id)
        {
            ProductGroup? group = null;
            var source = _dataContext.Groups.Include(g => g.Products).ThenInclude(p => p.Feedbacks).Where(g => g.DeleteDt == null);
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

            ShopGroupPageModel model = new() 
            { 
                ProductGroup = group, 
                Groups = source 
            };
            return View(model);
        }

        public IActionResult Product(String id)
        {
            Product? product = null;
            var source = _dataContext
                .Products
                .Where(p => p.DeleteDt == null)
                .Include(p => p.Feedbacks
                .Where(f => f.DeleteDt == null))
                .ThenInclude(f => f.User)
                .Include(p => p.Group)
                .ThenInclude(g => g.Products);

            product = source.FirstOrDefault(p => p.Slug == id);

            if (product == null)
            {
                product = source.FirstOrDefault(p => p.Id.ToString() == id);
            }

            if (product == null)
            {
                return View("Page404");
            }

            ShopProductPageModel model = new() 
            { 
                Product = product,
                ProductGroup = product.Group,
            };
            return View(model);
        }

        public IActionResult Cart()
        {
            return View();
        }
    }
}