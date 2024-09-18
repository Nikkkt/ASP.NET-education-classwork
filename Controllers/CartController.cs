using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Data.Entities;
using ASP.NET_Classwork.Models.Api;
using ASP.NET_Classwork.Models.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                    response.Meta.Count = 1;
                }
                else
                {
                    cartProduct.Count += formModel.Count;
                    response.Meta.Count = cartProduct.Count;
                }
            }

            await _dataContext.SaveChangesAsync();
            
            response.Data = "Added";

            return response;
        }

        [HttpGet]
        public RestResponse<Cart?> DoGet([FromQuery] String id)
        {
            RestResponse<Cart?> response = new()
            {
                Meta = new()
                {
                    Service = "Cart"
                },
                Data = _dataContext
                        .Carts
                        .Include(c => c.CartProducts)
                        .ThenInclude(cp => cp.Product)
                        .FirstOrDefault(
                            c => c.UserId.ToString() == id && 
                            c.CloseDt == null && 
                            c.DeleteDt == null
                        )
            };

            return response;
        }

        [HttpPut]
        public async Task<RestResponse<String>> DoPut([FromQuery] Guid cpId, [FromQuery] int increment)
        {
            RestResponse<String> response = new()
            {
                Meta = new()
                {
                    Service = "Cart"
                },
            };

            if (cpId == default)
            {
                response.Data = "Error 400: cpId is not valid";
                return response;
            }
            if (increment == 0)
            {
                response.Data = "Error 400: increment is not valid";
                return response;
            }

            var cp = _dataContext.CartProducts.Include(cp => cp.Cart).FirstOrDefault(cp => cp.Id == cpId);

            if (cp == null)
            {
                response.Data = "Error 404: cpId does not identify entity";
                return response;
            }
            if (cp.Cart.CloseDt is not null || cp.Cart.DeleteDt is not null)
            {
                response.Data = "Error 409: cpId identifies not active entity";
                return response;
            }
            if (cp.Count + increment < 0)
            {
                response.Data = "Error 422: increment could not be applied";
                return response;
            }

            if (cp.Count + increment == 0)
            {
                _dataContext.CartProducts.Remove(cp);
                response.Meta.Count = 0;
            }
            else
            {
                cp.Count += increment;
                response.Meta.Count = cp.Count;
            }

            await _dataContext.SaveChangesAsync();
            response.Data = "Updated";
            
            return response;
        }

        [HttpDelete]
        public async Task<RestResponse<String>> DoDelete([FromQuery] Guid cartId)
        {
            RestResponse<String> response = new()
            {
                Meta = new()
                {
                    Service = "Cart"
                },
            };

            if (cartId == default)
            {
                response.Data = "Error 400: cartId is not valid";
                return response;
            }

            var cart = _dataContext.Carts.FirstOrDefault(c => c.Id == cartId);
            if (cart == null) 
            { 
                response.Data = "Error 404: cartId is not found"; 
                return response; 
            }
            if (cart.CloseDt != null || cart.DeleteDt != null)
            {
                response.Data = "Error 409: cartId identify closed or deleted cart";
                return response;
            }

            cart.CloseDt = DateTime.Now;
            await _dataContext.SaveChangesAsync();
            response.Data = "Deleted";
            return response;
        }
    }
}
