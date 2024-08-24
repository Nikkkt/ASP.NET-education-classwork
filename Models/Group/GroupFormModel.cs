using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ASP.NET_Classwork.Models.Group
{
    public class GroupFormModel
    {
        [FromForm(Name = "group-name")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "group-description")]
        public String Description { get; set; } = null!;

        [FromForm(Name = "group-slug")]
        public String Slug { get; set; } = null!;

        [FromForm(Name = "group-picture")]
        [JsonIgnore]
        public IFormFile ImageFile { get; set; } = null!;
    }
}
