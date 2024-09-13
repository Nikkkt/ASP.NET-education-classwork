using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Models.Api;
using ASP.NET_Classwork.Models.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Classwork.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        [HttpPost]
        public async Task<RestResponse<String>> DoPost([FromBody] CartFormModel formModel)
        {
            RestResponse<String> response = new()
            {
                Meta = new()
                {
                    Service = "Cart"
                },
            };

            if (formModel.UserId == default) 
            {
                response.Data = "Error 401: Unauthorized";
                return response;
            }
            if (formModel.UserId == default)
            {
                response.Data = "Error 422: Missing Product Id";
                return response;
            }
            if (formModel.Count <= 0)
            {
                response.Data = "Error 422: Positive Count expected";
                return response;
            }

            var cart = _dataContext.Carts.FirstOrDefault(c => c.UserId == formModel.UserId && c.CloseDt == null && c.DeleteDt == null);

            if (cart == null)
            {
                Guid cartId = Guid.NewGuid();
                _dataContext.Carts.Add(new()
                {
                    Id = cartId,
                    UserId = formModel.UserId,
                    CreateDt = DateTime.Now,
                });
                _dataContext.CartProducts.Add(new()
                {
                    Id = Guid.NewGuid(),
                    CartId = cartId,
                    ProductId = formModel.ProductId,
                    Count = formModel.Count,
                });
            }
            else
            {
                var cartProduct = _dataContext.CartProducts.FirstOrDefault(cp => cp.CartId == cart.Id && cp.ProductId == formModel.ProductId);
                if (cartProduct == null) 
                {
                    _dataContext.CartProducts.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = formModel.ProductId,
                        Count = formModel.Count,
                    });
                }
                else
                {
                    cartProduct.Count += formModel.Count;
                }
            }

            await _dataContext.SaveChangesAsync();
            response.Data = "Added";

            return response;
        }
    }
}
