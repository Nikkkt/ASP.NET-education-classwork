using ASP.NET_Classwork.Data.Entities;

namespace ASP.NET_Classwork.Models.Shop
{
    public class ShopProductPageModel
    {
        public Data.Entities.Product Product { get; set; } = null!;
        public ProductGroup ProductGroup { get; set; } = null!;
    }
}
