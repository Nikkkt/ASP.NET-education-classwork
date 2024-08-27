using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ASP.NET_Classwork.Models.Shop
{
    public class ShopProductFormModel
    {
        [FromForm(Name = "product-name")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "product-description")]
        public String Description { get; set; } = null!;

        [FromForm(Name = "product-slug")]
        public String? Slug { get; set; }

        [FromForm(Name = "product-picture")]
        [JsonIgnore]
        public IFormFile ImageFile { get; set; } = null!;

        [FromForm(Name = "product-price")]
        public double Price { get; set; }

        [FromForm(Name = "product-amount")]
        public int Amount { get; set; }

        [FromForm(Name = "group-id")]
        public Guid GroupId { get; set; }
    }
}
